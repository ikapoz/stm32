#include "interrupts.hpp"
#include "interrupt_mgr.hpp"

void default_handler(void) {
  while (1) {
  }
}

void dummy_handler() {
  int i = 0;
  ++i;
}

void nmi_handler() __attribute__((weak, alias("default_handler")));
void hard_fault_handler() __attribute__((weak, alias("default_handler")));
void mem_manage_handler() __attribute__((weak, alias("default_handler")));
void bus_manage_handler() __attribute__((weak, alias("default_handler")));
void usage_fault_handler() __attribute__((weak, alias("default_handler")));

void svc_handler() __attribute__((weak, alias("dummy_handler")));
void debug_mon_handler() __attribute__((weak, alias("dummy_handler")));

void sys_tick_handler() __attribute__((weak, alias("dummy_handler")));