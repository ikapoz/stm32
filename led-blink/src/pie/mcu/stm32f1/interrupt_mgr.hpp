#pragma once
#include "Core/Inc/stm32_assert.h"
#include "interrupts.hpp"
#include "pie/container/vector.hpp"
#include "pie/include/compiler.h"
#include <stdint.h>

struct exception_stack_frame {
  uint32_t r0;
  uint32_t r1;
  uint32_t r2;
  uint32_t r3;
  uint32_t r12;
  uint32_t lr;
  uint32_t pc;
  uint32_t psr;
};

// function visible in C and ASM
extern "C" {
void exception_handler_c(exception_stack_frame *frame PIE_ATTR_UNUSED,
                         uint32_t lr PIE_ATTR_UNUSED, uint32_t exception_type);
void hard_fault_handler();
void bus_fault_handler();
void usage_fault_handler();
}

namespace pie {

namespace interrupts {
__attribute__((always_inline)) inline void enable() {
  __asm volatile("cpsie i");
}
__attribute__((always_inline)) inline void disable() {
  __asm volatile("cpsid i");
}

} // namespace interrupts

typedef uint32_t *Address;
typedef void (*Callback)(Address);
template <class W> W &reference_to(Address pw) {
  return *static_cast<W *>((void *)pw);
}
template <typename func> Address get_address(func &f) {
  return static_cast<Address>(f);
}

enum class Event_id : uint8_t {
  no_event,
  /* IRQs */
  sys_tick,
  wwdg, // watch dog timer
  // EXTI_0_IRQ,
  // EXTI_1_IRQ,
  // EXTI_2_IRQ,
  // EXTI_3_IRQ,
  // EXTI_4_IRQ,
  // DMA1_Channel2_IRQ,
  // DMA1_Channel3_IRQ,
  // DMA1_Channel4_IRQ,
  // DMA1_Channel5_IRQ,
  // DMA1_Channel6_IRQ,
  // DMA1_Channel7_IRQ,
  // USB_CAN_IRQ,
  // USB_HP_CAN1_TX_IRQ,
  // USB_LP_CAN1_RX0_IRQ,
  // EXTI_9_5_IRQ,
  // TIM_2_IRQ
  // TIM_4_IRQ,
  // I2C1_ev_IRQ,
  // I2C1_er_IRQ
  // USART_1_IRQ,
  // USB_WakeUp_IRQ,
  LED_IRQ
};

class Event_task {
public:
  virtual void execute_event(pie::Event_id id) = 0;
};

// class Event {
// public:
//   Event(Address event_task_address)
//       : event_task{reference_to<Event_task>(event_task_address)},
//         event_task_address{event_task_address} {}
//   Event() : event_task{reference_to<Event_task>(event_task_address)} {}
//   // Event(const Event &); // copy constructor
//   Event &operator=(const Event &event); // copy assignment
//   bool is_enabled() const { return is_enabled_; }
//   void enable() { is_enabled_ = true; }
//   void disable() { is_enabled_ = false; }
//   void execute(Event_id id) {
//     if (event_task_address)
//       event_task.execute_event(id);
//   };

// protected:
//   Event_task &event_task;
//   Address event_task_address{0};

// private:
//   bool is_enabled_{true};
// };

class Interrupt_mng {
public:
  // Interrupt_mng();
  /**
   * @brief Interrupt manager. Takes care on publishing events to subscriber
   * @param vtor_flash_address FLASH address where interrupt vector table resist
   * @param vtor_ram_address RAM memory where interrupt vector table should be
   * copied and it also add system specific interrupts. memory address should be
   * alligned (see documentation for details)
   */
  Interrupt_mng(uint32_t vtor_flash_address, uint32_t vtor_ram_address);
  inline void on_event(Event_id event_id) {
    interrupts::disable();
    for (uint8_t i = 0; i < end_pos; i++)
      subscribed[i]->execute_event(event_id);

    interrupts::enable();
  }

  void subscribe(Event_task *event_task) {
    assert_param(!full());
    subscribed[end_pos] = event_task;
    ++end_pos;
  }

private:
  static const uint32_t SIZE = 10;
  Event_task *subscribed[SIZE];
  uint8_t end_pos{0};
  // uint8_t free_elements_count; /*!< Counts number of free elements */

  bool full() { return SIZE == end_pos; }
};

void sys_tick_handler();

// void EXTI_0_IRQ_handler();
// void EXTI_1_IRQ_handler();
// void EXTI_2_IRQ_handler();
// void EXTI_3_IRQ_handler();
// void EXTI_4_IRQ_handler();

// void DMA1_channel_2_IRQ_handler();
// void DMA1_channel_3_IRQ_handler();
// void DMA1_channel_4_IRQ_handler();
// void DMA1_channel_5_IRQ_handler();
// void DMA1_channel_6_IRQ_handler();
// void DMA1_channel_7_IRQ_handler();

// void USB_CAN_IRQ_handler();
// void EXTI_9_5_IRQ_handler();
void TIM_2_IRQHandler();
// void TIM_4_IRQHandler();
// void I2C1_ev_IRQ_handler();
// void I2C1_er_IRQ_handler();
// void USART_1_IRQ_handler();
// void USB_Wakeup_IRQ_handler();
} // namespace pie

// declaring global interrupt manager
extern "C" pie::Interrupt_mng *g_interrupt_mng;