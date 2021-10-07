#include "boot.h"
#include "Core/Inc/main.h"
//#include "main.hpp"
#include "stm32f1xx.h"
#include <main.hpp>
#include <stdint.h>
/* These are defined in the linker script */
extern uint32_t _stext;
extern uint32_t _etext;
extern uint32_t _sbss;
extern uint32_t _ebss;
extern uint32_t _sdata;
extern uint32_t _edata;
extern uint32_t _sstack;
extern uint32_t _estack;
extern uint32_t _flash_data;
extern uint32_t _flash_bss;

#define SRAM_SIZE 20 * 1024 // STM32F103C8 has 20 KB of RAM
/* Work out end of RAM address as initial stack pointer */
#define SRAM_END (SRAM_BASE + SRAM_SIZE)

/* FLASH VTOR Table */
__attribute__((section(".vectors"))) uint32_t *vector_table[] = {
    /* Stack pointer */
    (uint32_t *)SRAM_END,
    /* Cortex-M handlers */
    (uint32_t *)&reset_handler,
    /* end region */
    (uint32_t *)END_REGION};

void system_init();

/**
 * This is the code that gets called on processor reset.
 * To initialize the device, and call the main() routine.
 */
__attribute__((noreturn, weak)) void reset_handler(void) {

  /* Copy data from FLASH to RAM  */
  uint32_t *src = &_flash_data;
  uint32_t *dst = &_sdata; // &_edata;
  uint32_t *dst_end = &_edata;

  while (dst < dst_end) {
    *dst = *src;
    ++dst;
    ++src;
  }

  // Zero bss
  dst = &_sbss;
  dst_end = &_ebss;
  while (dst < dst_end) {
    *dst = 0;
    ++dst;
  }

  /* STM32Cube main */
  /* Used only to setup hardware */
  main();
  
  /* Branch to main function */
  app_main();

  /* Infinite loop */
  /* we should never get here */
  while (1)
    ;
}