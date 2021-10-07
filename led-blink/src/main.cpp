#include "main.hpp"
#include "boot.h"
#include "pie/mcu/stm32f1/device/led.hpp"
#include "pie/mcu/stm32f1/interrupt_mgr.hpp"
#include "pie/mcu/stm32f1/utils.hpp"
int app_main() {
  pie::Interrupt_mng interrupt_mng{FLASH_BASE, SRAM_BASE};
  pie::device::Led led{interrupt_mng, LED_GPIO_Port, LED_Pin, TIM2};
  // setup all system
  // !!! If CPU clock frequency is changed update this !!!
  pie::utils::init_tick(72000000, 1000); /// < HCLKFrequency
  led.start();
  pie::interrupts::enable();
  while (true) {
#if !DEBUG
    pie::power::sleep_wfi();
#endif
  }
}