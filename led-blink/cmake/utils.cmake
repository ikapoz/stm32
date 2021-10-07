# Add custom command to print firmware size in Berkley format
function(firmware_size target)
    add_custom_command(TARGET ${target} POST_BUILD
        COMMAND ${CMAKE_SIZE_UTIL} -B
        "${CMAKE_CURRENT_BINARY_DIR}/${target}${CMAKE_EXECUTABLE_SUFFIX}"
    )
endfunction()

function(objdump target)
    add_custom_command(TARGET ${target} POST_BUILD
        COMMAND ${CMAKE_OBJDUMP} -D -S
        "${CMAKE_CURRENT_BINARY_DIR}/${target}" > "${CMAKE_CURRENT_BINARY_DIR}/${target}.list"
    )
endfunction()

# Add a command to generate firmware in a provided format
function(generate_object target suffix type strip)
if(NOT strip)
    add_custom_command(TARGET ${target} POST_BUILD
        COMMAND ${CMAKE_OBJCOPY} -O ${type}
        "${CMAKE_CURRENT_BINARY_DIR}/${target}${CMAKE_EXECUTABLE_SUFFIX}" "${CMAKE_CURRENT_BINARY_DIR}/${target}${suffix}")
else()
    add_custom_command(TARGET ${target} POST_BUILD
        COMMAND ${CMAKE_OBJCOPY} -O ${type} -S
        "${CMAKE_CURRENT_BINARY_DIR}/${target}${CMAKE_EXECUTABLE_SUFFIX}" "${CMAKE_CURRENT_BINARY_DIR}/${target}${suffix}")
endif()
endfunction()
 

macro(pie_set_default_compiler_options target mcu) 
        target_compile_options(${target} PUBLIC        
            $<$<COMPILE_LANGUAGE:CXX>:
            --specs=nano.specs
           >)
            target_compile_options(${target} PUBLIC
            ${mcu}
            --specs=nano.specs
            -fverbose-asm
            -ffunction-sections
            -fdata-sections
            -fstack-usage)
endmacro()

macro (_pie_add_compiler_flags cmake_flags options)
    foreach (flag ${cmake_flags})
        set (new_flags ${${flag}} ${options})
        string (REPLACE ";" " " ${flag} "${new_flags}")
    endforeach ()
endmacro ()

macro (pie_add_cxx_flags)
_pie_add_compiler_flags ("CMAKE_CXX_FLAGS" "${ARGN}")
endmacro ()

macro (pie_add_c_flags)
_pie_add_compiler_flags ("CMAKE_C_FLAGS" "${ARGN}")
endmacro ()

macro (pie_add_c_cxx_flags)
    pie_add_c_flags (${ARGN})
    pie_add_cxx_flags (${ARGN})
endmacro ()

macro (pie_disable_exceptions)
    if (CMAKE_COMPILER_IS_GNUCC)
        pie_add_cxx_flags("-fno-exceptions -fno-unwind-tables")
    endif ()
endmacro ()

macro (pie_disable_rtti)
    if (CMAKE_COMPILER_IS_GNUCC)
        pie_add_cxx_flags("-fno-rtti")
    endif ()
endmacro ()

macro (pie_disable_stdlib)
    add_definitions(-DNOSTDLIB)
    if (CMAKE_COMPILER_IS_GNUCC)
        pie_add_c_cxx_flags("-nostdlib")
    endif ()
endmacro ()