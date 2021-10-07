#pragma once
#include <stdint.h>
#ifdef __cplusplus
extern "C" {
#endif
void dummy_handler();
void default_handler();

void nmi_handler();
void hard_fault_handler();
void mem_manage_handler();
void bus_manage_handler();
void usage_fault_handler();

void svc_handler();
void debug_mon_handler();

void sys_tick_handler();



struct Vector_table {
  /* Stack pointer */
  uint32_t *pvStack;

  /* Cortex-M handlers */
  uint32_t *pfnReset_Handler;
  uint32_t *pfnNMI_Handler;
  uint32_t *pfnHardFault_Handler;
  uint32_t *pfnMemManage_Handler;
  uint32_t *pfnBusFault_Handler;
  uint32_t *pfnUsageFault_Handler;
  uint32_t *pvReservedM7;
  uint32_t *pvReservedM8;
  uint32_t *pvReservedM9;
  uint32_t *pvReservedM10;
  uint32_t *pfnSVC_Handler;
  uint32_t *pfnDebugMon_Handler;
  uint32_t *pvReservedM13;
  uint32_t *pfnPendSV_Handlerl;
  uint32_t *pfnSysTick_Handler;

  uint32_t *pfnWWDG_IRQHandler; /*  0 World Watchdog Timer */
  uint32_t *pfnPVD_IRQHandler;
  uint32_t *pfnTAMPER_IRQHandler;
  uint32_t *pfnRTC_IRQHandler; /*  3 Real-Time Counter */
  uint32_t *pfnFLASH_IRQHandler;
  uint32_t *pfnRCC_IRQHandler;
  uint32_t *pfnEXTI0_IRQHandler;
  uint32_t *pfnEXTI1_IRQHandler;
  uint32_t *pfnEXTI2_IRQHandler;
  uint32_t *pfnEXTI3_IRQHandler;
  uint32_t *pfnEXTI4_IRQHandler;
  uint32_t *pfnDMA1_Channel1_IRQHandler;
  uint32_t *pfnDMA1_Channel2_IRQHandler;
  uint32_t *pfnDMA1_Channel3_IRQHandler;
  uint32_t *pfnDMA1_Channel4_IRQHandler;
  uint32_t *pfnDMA1_Channel5_IRQHandler;
  uint32_t *pfnDMA1_Channel6_IRQHandler;
  uint32_t *pfnDMA1_Channel7_IRQHandler;
  uint32_t *pfnADC1_2_IRQHandler;
  uint32_t *pfnUSB_HP_CAN1_TX_IRQHandler;
  uint32_t *pfnUSB_LP_CAN1_RX0_IRQHandler;
  uint32_t *pfnCAN1_RX1_IRQHandler;
  uint32_t *pfnCAN1_SCE_IRQHandler;
  uint32_t *pfnEXTI9_5_IRQHandler;
  uint32_t *pfnTIM1_BRK_IRQHandler;
  uint32_t *pfnTIM1_UP_IRQHandler;
  uint32_t *pfnTIM1_TRG_COM_IRQHandler;
  uint32_t *pfnTIM1_CC_IRQHandler;
  uint32_t *pfnTIM2_IRQHandler;
  uint32_t *pfnTIM3_IRQHandler;
  uint32_t *pfnTIM4_IRQHandler;
  uint32_t *pfnI2C1_EV_IRQHandler;
  uint32_t *pfnI2C1_ER_IRQHandler;
  uint32_t *pfnI2C2_EV_IRQHandler;
  uint32_t *pfnI2C2_ER_IRQHandler;
  uint32_t *pfnSPI1_IRQHandler;
  uint32_t *pfnSPI2_IRQHandler;
  uint32_t *pfnUSART1_IRQHandler;
  uint32_t *pfnUSART2_IRQHandler;
  uint32_t *pfnUSART3_IRQHandler;
  uint32_t *pfnEXTI15_10_IRQHandler;
  uint32_t *pfnRTC_Alarm_IRQHandler;
  uint32_t *pfnUSBWakeUp_IRQHandler;
  uint32_t *pvReserved59;
  uint32_t *pvReserved60;
  uint32_t *pvReserved61;
  uint32_t *pvReserved62;
  uint32_t *pvReserved63;
  uint32_t *pvReserved64;
  uint32_t *pvReserved65;
  /* if there is a need to use this handler please move RAM region in linker
  script. Currently is an offset of 64 uint32_t */
  uint32_t *pfnBootRAM; /* @0x108.
 This is for boot in RAM mode for; STM32F10x Medium Density devices. */
};
#ifdef __cplusplus
}
#endif