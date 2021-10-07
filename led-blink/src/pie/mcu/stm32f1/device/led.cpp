#include "led.hpp"

namespace pie {
namespace device {

void Led::execute_event(pie::Event_id id) {
  switch (id) {
  case pie::Event_id::LED_IRQ: {
    LL_GPIO_TogglePin(gpio_port, pin_mask);
    LL_TIM_ClearFlag_UPDATE(timer);
    break;
  }
  default:
    break;
  }
}

Led::~Led() {}

void Led::start() {
  LL_TIM_EnableIT_UPDATE(timer);
  LL_TIM_EnableUpdateEvent(timer);
  LL_TIM_EnableCounter(timer);
}

} // namespace device
} // namespace pie