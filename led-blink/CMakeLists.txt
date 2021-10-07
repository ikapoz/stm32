set (CMAKE_MODULE_PATH "${CMAKE_MODULE_PATH};${CMAKE_CURRENT_SOURCE_DIR}/cmake")
cmake_minimum_required(VERSION 3.12..3.17)

if(${CMAKE_VERSION} VERSION_LESS 3.17)
    cmake_policy(VERSION ${CMAKE_MAJOR_VERSION}.${CMAKE_MINOR_VERSION})
endif()

include(utils)

project(first_app
    VERSION 1.0
    DESCRIPTION "STM32F103 LED project"
    LANGUAGES CXX ASM C)

# Let's nicely support folders in IDEs
set_property(GLOBAL PROPERTY USE_FOLDERS ON)

set(CPU cortex-m3)
set(FLOAT_ABI soft)
set(MCU -mcpu=${CPU} -mthumb -mfloat-abi=${FLOAT_ABI})
set(TARGET main)
set(STARTUP_LIB_TARGET startup_lib)

# Source files added to project
add_executable(${TARGET}
    src/Core/Src/main.c 
    src/Core/Src/stm32f1xx_it.c 
    src/Core/Src/system_stm32f1xx.c     
    src/Drivers/STM32F1xx_HAL_Driver/Src/stm32f1xx_ll_dma.c 
    src/Drivers/STM32F1xx_HAL_Driver/Src/stm32f1xx_ll_exti.c 
    src/Drivers/STM32F1xx_HAL_Driver/Src/stm32f1xx_ll_gpio.c 
    src/Drivers/STM32F1xx_HAL_Driver/Src/stm32f1xx_ll_rcc.c 
    src/Drivers/STM32F1xx_HAL_Driver/Src/stm32f1xx_ll_tim.c 
    src/Drivers/STM32F1xx_HAL_Driver/Src/stm32f1xx_ll_utils.c 
    src/pie/mcu/stm32f1/interrupt_mgr.cpp
    src/pie/mcu/stm32f1/interrupts.cpp
    src/pie/mcu/stm32f1/device/led.cpp
    src/pie/util/stub.cpp
    src/main.cpp
    src/pie/mcu/stm32f1/boot.c)

# asembler file added compile flags
#set_source_files_properties(startup_stm32f103xb.s PROPERTIES COMPILE_FLAGS "-x assembler-with-cpp")

generate_object(${TARGET} .hex ihex FALSE)
generate_object(${TARGET} .bin binary TRUE)
objdump(${TARGET})
firmware_size(${TARGET})

# C, CXX compile, debug properties and include folders
set_target_properties(${TARGET} PROPERTIES CXX_STANDARD 20)
target_compile_definitions(${TARGET} PUBLIC 
                    STM32F103xB #STM32 hardware
                    USE_FULL_LL_DRIVER #CUBE driver usage LL 
                    #USE_HALL_DRIVER
                    HSE_VALUE=8000000
                    HSE_STARTUP_TIMEOUT=100
                    LSE_STARTUP_TIMEOUT=5000
                    LSE_VALUE=32768
                    HSI_VALUE=8000000
                    LSI_VALUE=40000
                    VDD_VALUE=3300
                    PREFETCH_ENABLE=1)     
       
pie_set_default_compiler_options(${TARGET} ${MCU})
pie_disable_exceptions()
pie_disable_rtti()
pie_disable_stdlib()

if(${CMAKE_BUILD_TYPE} MATCHES  Debug) 
    target_compile_definitions(${TARGET} PUBLIC 
                    DEBUG
                    USE_FULL_ASSERT=1U)
    target_compile_options(${TARGET} PUBLIC
                    -Wall)
endif()

#add_library(${STARTUP_LIB_TARGET} startup_stm32f103xb)

#target_link_libraries(${TARGET} PRIVATE stdlib_stub_lib)

target_link_options(${TARGET} PUBLIC
                ${MCU}
                -T${CMAKE_CURRENT_SOURCE_DIR}/STM32F103C8Tx_FLASH.ld
                -specs=nano.specs
                -Wl,-Map=${CMAKE_CURRENT_BINARY_DIR}/${TARGET}.map,--cref 
                -Wl,--gc-sections
                #-Wl,--verbose
                )

target_include_directories(${TARGET} SYSTEM PUBLIC
               "src/drivers"
               "src/drivers/STM32F1xx_HAL_Driver/Inc"
               "src/drivers/CMSIS/Device/ST/STM32F1xx/Include"
               "src/drivers/CMSIS/Include")      

target_include_directories(${TARGET} PUBLIC
                                    "src/pie"
                                    "src/pie/mcu/stm32f1"
                                    "src/pie/mcu/stm32f1/device"
                                    "src/Core/Inc"
                                    "src")      