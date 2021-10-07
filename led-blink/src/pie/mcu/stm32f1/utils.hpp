#pragma once
#include "Core/Inc/stm32_assert.h"
#include "pie/include/compiler.h"
#include "stm32f1xx.h"
#include <stdint.h>
namespace pie {
namespace utils {
/**
 * @brief  This function configures the Cortex-M SysTick source of the time
 *         base.
 * @param HCLK_frequency HCLK frequency in Hz (can be calculated thanks to RCC
 *                       helper macro)
 * @note   When a RTOS is used, it is recommended to avoid changing the SysTick
 *         configuration by calling this function, for a delay use rather
 *         osDelay RTOS service.
 * @param  Ticks Number of ticks
 */
PIE_STATIC_INLINE void init_tick(uint32_t HCLK_frequency, uint32_t ticks) {
  auto value = (uint32_t)((HCLK_frequency / ticks) - 1UL);
  assert_param(value > SysTick_LOAD_RELOAD_Msk);
  SysTick->LOAD = value;
  SysTick->VAL = 0UL;
  SysTick->CTRL =
      SysTick_CTRL_CLKSOURCE_Msk | SysTick_CTRL_TICKINT_Msk |
      SysTick_CTRL_ENABLE_Msk; /* Enable SysTick IRQ and SysTick Timer */
}
} // namespace utils
namespace power {
/**
 * Enter sleep mode and WFI (wait for interrupt)
 */
inline void sleep_wfi() { __asm volatile("wfi"); }
/**
 * Enter sleep mode and WFE (wait for event)
 */
inline void sleep_wfe() { __asm volatile("wfe"); }
} // namespace power
} // namespace pie