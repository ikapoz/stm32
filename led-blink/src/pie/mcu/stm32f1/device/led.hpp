#pragma once
#include "Core/Inc/main.h"
#include "interrupt_mgr.hpp"

//#include "../interrupt_mgr.hpp"

namespace pie {
namespace device {
class Led : public virtual pie::Event_task {
public:
  Led(pie::Interrupt_mng &interrupt_mng, GPIO_TypeDef *gpio_port,
      uint32_t pin_mask, TIM_TypeDef *timer)
      : interrupt_mng{interrupt_mng}, gpio_port{gpio_port}, pin_mask{pin_mask},
        timer{timer} {
    interrupt_mng.subscribe((pie::Event_task *)pie::Address(this));
  }
  ~Led();
  void execute_event(pie::Event_id id);
  void start();

private:
  pie::Interrupt_mng &interrupt_mng;
  GPIO_TypeDef *gpio_port;
  uint32_t pin_mask;
  TIM_TypeDef *timer;
};

} // namespace device
} // namespace pie