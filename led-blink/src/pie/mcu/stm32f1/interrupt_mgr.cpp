#include "interrupt_mgr.hpp"
#include "interrupts.hpp"
#include "pie/mcu/stm32f1/boot.h"
#include <memory>
#include <stdint.h>

// define global pointer
pie::Interrupt_mng *g_interrupt_mng = nullptr;

// Hard Fault handler wrapper in assembly.
// It extracts the location of stack frame and passes it to handler
// in C as a pointer. We also pass the LR value as second
// parameter.
// (Based on Joseph Yiu's, The Definitive Guide to ARM Cortex-M3 and
// Cortex-M4 Processors, Third Edition, Chap. 12.8, page 402).

// void exception_handler() {
//   asm volatile("tst lr,#4              \n"
//                "ite eq                 \n"
//                "mrseq r0,msp           \n"
//                "mrsne r0,psp           \n"
//                "mov r1,lr              \n"
//                "ldr r2,=exception_handler_c \n"
//                "bx r2");
// }

void hard_fault_handler() {
  asm volatile("tst lr,#4              \n"
               "ite eq                 \n"
               "mrseq r0,msp           \n"
               "mrsne r0,psp           \n"
               "mov r1,lr              \n"
               "mov r2,0               \n"
               "ldr r3,=exception_handler_c \n"
               "bx r3");
}

void bus_fault_handler() {
  asm volatile("tst lr,#4              \n"
               "ite eq                 \n"
               "mrseq r0,msp           \n"
               "mrsne r0,psp           \n"
               "mov r1,lr              \n"
               "mov r2,1               \n"
               "ldr r3,=exception_handler_c \n"
               "bx r3");
}

/**
 * exception type set to 1=
 */
void usage_fault_handler() {
  asm volatile("tst lr,#4              \n"
               "ite eq                 \n"
               "mrseq r0,msp           \n"
               "mrsne r0,psp           \n"
               "mov r1,lr              \n"
               "mov r2,2               \n"
               "ldr r3,=exception_handler_c \n"
               "bx r3");
}

void exception_handler_c(exception_stack_frame *frame PIE_ATTR_UNUSED,
                         uint32_t lr PIE_ATTR_UNUSED, uint32_t exception_type) {
  asm volatile("bkpt 0");
}

pie::Interrupt_mng::Interrupt_mng(uint32_t vtor_flash_address,
                                  uint32_t vtor_ram_address) {
  // copy from base VTOR to RAM
  g_interrupt_mng = this;
  uint32_t *src = (uint32_t *)vtor_flash_address;
  uint32_t *dst = (uint32_t *)vtor_ram_address;
  uint32_t i = 0;
  auto is_end_region = false;
  do {
    if (src[i] == END_REGION)
      is_end_region = true;

    is_end_region ? dst[i] = 0 : dst[i] = src[i];
    ++i;
  } while (i < 64);

  auto vtor = (Vector_table *)vtor_ram_address;
  vtor->pfnHardFault_Handler = Address(hard_fault_handler);
  vtor->pfnBusFault_Handler = Address(bus_fault_handler);
  vtor->pfnUsageFault_Handler = Address(usage_fault_handler);
  vtor->pfnSysTick_Handler = Address(pie::sys_tick_handler);
  // vtor->pfnEXTI0_IRQHandler = Address(pie::EXTI_0_IRQ_handler);
  // vtor->pfnEXTI1_IRQHandler = Address(pie::EXTI_1_IRQ_handler);
  // vtor->pfnEXTI2_IRQHandler = Address(pie::EXTI_2_IRQ_handler);
  // vtor->pfnEXTI3_IRQHandler = Address(pie::EXTI_3_IRQ_handler);
  // vtor->pfnEXTI4_IRQHandler = Address(pie::EXTI_4_IRQ_handler);
  // vtor->pfnDMA1_Channel2_IRQHandler =
  // Address(pie::DMA1_channel_2_IRQ_handler); vtor->pfnDMA1_Channel3_IRQHandler
  // = Address(pie::DMA1_channel_3_IRQ_handler);
  // vtor->pfnDMA1_Channel4_IRQHandler =
  // Address(pie::DMA1_channel_4_IRQ_handler); vtor->pfnDMA1_Channel5_IRQHandler
  // = Address(pie::DMA1_channel_5_IRQ_handler);
  // vtor->pfnEXTI9_5_IRQHandler = Address(pie::EXTI_9_5_IRQ_handler);
  vtor->pfnTIM2_IRQHandler = Address(pie::TIM_2_IRQHandler);
  // vtor->pfnTIM4_IRQHandler = Address(pie::TIM_4_IRQHandler);
  // vtor->pfnUSB_HP_CAN1_TX_IRQHandler = Address(pie::USB_CAN_IRQ_handler);
  // vtor->pfnUSB_LP_CAN1_RX0_IRQHandler = Address(pie::USB_CAN_IRQ_handler);
  // vtor->pfnUSBWakeUp_IRQHandler = Address(pie::USB_Wakeup_IRQ_handler);
  // vtor->pfnI2C1_ER_IRQHandler = Address(pie::I2C1_er_IRQ_handler);
  // vtor->pfnI2C1_EV_IRQHandler = Address(pie::I2C1_ev_IRQ_handler);
  // vtor->pfnUSART1_IRQHandler = Address(pie::USART_1_IRQ_handler);
  // realocate VTOR table to RAM
  SCB->VTOR = vtor_ram_address;
}

void pie::sys_tick_handler() {
  if (g_interrupt_mng)
    g_interrupt_mng->on_event(pie::Event_id::sys_tick);
}

// void pie::EXTI_0_IRQ_handler() {
//   if (g_interrupt_mng)
//     g_interrupt_mng->on_event(pie::Event_id::EXTI_0_IRQ);
// }
// void pie::EXTI_1_IRQ_handler() {
//   if (g_interrupt_mng)
//     g_interrupt_mng->on_event(pie::Event_id::EXTI_1_IRQ);
// }
// void pie::EXTI_2_IRQ_handler() {
//   if (g_interrupt_mng)
//     g_interrupt_mng->on_event(pie::Event_id::EXTI_2_IRQ);
// }

// void pie::EXTI_3_IRQ_handler() {
//   if (g_interrupt_mng)
//     g_interrupt_mng->on_event(pie::Event_id::EXTI_3_IRQ);
// }

// void pie::EXTI_4_IRQ_handler() {
//   if (g_interrupt_mng)
//     g_interrupt_mng->on_event(pie::Event_id::EXTI_4_IRQ);
// }

// void pie::DMA1_channel_2_IRQ_handler() {
//   if (g_interrupt_mng)
//     g_interrupt_mng->on_event(pie::Event_id::DMA1_Channel2_IRQ);
// }

// void pie::DMA1_channel_3_IRQ_handler() {
//   if (g_interrupt_mng)
//     g_interrupt_mng->on_event(pie::Event_id::DMA1_Channel3_IRQ);
// }

// void pie::DMA1_channel_4_IRQ_handler() {
//   if (g_interrupt_mng)
//     g_interrupt_mng->on_event(pie::Event_id::DMA1_Channel4_IRQ);
// }

// void pie::DMA1_channel_5_IRQ_handler() {
//   if (g_interrupt_mng)
//     g_interrupt_mng->on_event(pie::Event_id::DMA1_Channel5_IRQ);
// }

// void pie::DMA1_channel_6_IRQ_handler() {
//  if (g_interrupt_mng)
//    g_interrupt_mng->on_event(pie::Event_id::DMA1_Channel6_IRQ);
//}
//
// void pie::DMA1_channel_7_IRQ_handler() {
//  if (g_interrupt_mng)
//    g_interrupt_mng->on_event(pie::Event_id::DMA1_Channel7_IRQ);
//}

// void pie::USB_CAN_IRQ_handler() {
//   if (g_interrupt_mng)
//     g_interrupt_mng->on_event(pie::Event_id::USB_CAN_IRQ);
// }

// void pie::EXTI_9_5_IRQ_handler() {
//   if (g_interrupt_mng)
//     g_interrupt_mng->on_event(pie::Event_id::EXTI_9_5_IRQ);
// }

void pie::TIM_2_IRQHandler() {
  if (g_interrupt_mng)
    g_interrupt_mng->on_event(pie::Event_id::LED_IRQ);
}

// void pie::TIM_4_IRQHandler() {
//   if (g_interrupt_mng)
//     g_interrupt_mng->on_event(pie::Event_id::TIM_4_IRQ);
// }

// void pie::I2C1_ev_IRQ_handler() {
//  if (g_interrupt_mng)
//    g_interrupt_mng->on_event(pie::Event_id::I2C1_ev_IRQ);
//}
//
// void pie::I2C1_er_IRQ_handler() {
//  if (g_interrupt_mng)
//    g_interrupt_mng->on_event(pie::Event_id::I2C1_er_IRQ);
//}

// void pie::USART_1_IRQ_handler() {
//   if (g_interrupt_mng)
//     g_interrupt_mng->on_event(pie::Event_id::USART_1_IRQ);
// }

// void pie::USB_Wakeup_IRQ_handler() {
//   if (g_interrupt_mng)
//     g_interrupt_mng->on_event(pie::Event_id::USB_CAN_IRQ);
// }

// pie::Event_task::~Event_task() {}