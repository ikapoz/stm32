/* Entry Point */
ENTRY(reset_handler)

/* Highest address of the user mode stack */
/* _estack = 0x20005000;  */   /* end of RAM */

/* Generate a link error if heap and stack don't fit into RAM */
_Min_Heap_Size = 0x200; /* required amount of heap  */
_Min_Stack_Size = 0x400; /* required amount of stack */
_vtor_size = 0x100; /* place for 64 interupts */
MEMORY
{
    RAM (xrw)       : ORIGIN = 0x20000100, LENGTH = 20K - _vtor_size
    FLASH (rx)      : ORIGIN = 0x8000000, LENGTH = 64K
}

/* Highest address of the user mode stack */
_estack = ORIGIN(RAM) + LENGTH(RAM); /* - _vtor_size; /* end of "RAM" Ram type memory */

SECTIONS
{
    /* The program code and other data into "FLASH" Rom type memory */
    .text : {
        . = ALIGN(4);
        _stext = .;
        KEEP (*(.vectors .vectors.*))
        *(.text .text.*)
        *(.rodata .rodata.*)   /* .rodata sections (constants, strings, etc.) */
        KEEP (*(.init))
        KEEP (*(.fini))
        . = ALIGN(4);
        _etext = .;        /* define a global symbols at end of code */
    } > FLASH

    .gnu_build_id :
    {
        PROVIDE(g_gnu_build_id = .);
        *(.note.gnu.build-id)
    } > FLASH

    /* .ARM.exidx is sorted, so has to go in its own output section.  */
    PROVIDE_HIDDEN (__exidx_start = .);
    .ARM.exidx :
    {
      *(.ARM.exidx* .gnu.linkonce.armexidx.*)
    } > FLASH
    PROVIDE_HIDDEN (__exidx_end = .);

    /* Initialized data sections into "RAM" Ram type memory */
    _flash_data = LOADADDR(.data);
    .data : {
        . = ALIGN(4);
        _sdata = .;  /* create a global symbol at data start */
        *(.data)
        *(.data*)
        . = ALIGN(4);
        _edata = .;  /* define a global symbol at data end */
    } > RAM AT > FLASH

    /* Uninitialized data section into "RAM" Ram type memory */
    .bss (NOLOAD) : {
        . = ALIGN(4);
        _sbss = . ;
        *(.bss)
        *(.bss.*)
        *(COMMON)
        . = ALIGN(4);
        _ebss = . ;
    } > RAM AT > FLASH

    /* stack section */
    /* User_heap_stack section, used to check that there is enough RAM left */
    .stack (NOLOAD): {
        . = ALIGN(8);
        _end = . ; /* create a global symbol at heap start */
        . = . + _Min_Heap_Size;
        _sstack = .;
        . = . + _Min_Stack_Size;
        . = ALIGN(8);
    } > RAM

    /* Remove information from the compiler libraries */
    /DISCARD/ : {
      libc.a ( * )
      libm.a ( * )
      libgcc.a ( * )
    }

    .ARM.attributes 0 : { *(.ARM.attributes) }
}