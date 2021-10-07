#pragma once
namespace pie {
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