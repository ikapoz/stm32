#pragma once
#include <stdint.h>
namespace pie {

/// @brief bitset_32.
/// @details bitset of maximal size of 32 bits
/// @headerfile pie/container/bitset.hpp
class bitset_32 {
public:
  /// @brief bitset_32.
  /// @details bitset of maximal size of 32 bits
  /// @tparam size total bits that will be used
  /// @headerfile pie/container/bitset.hpp
  bitset_32(uint32_t size) : size{size} {}
  inline void set(uint32_t bit_position) {
    auto bit = 1U << bit_position;
    bits |= bit;
  }
  inline void set() { bits = ~0; }

  inline void reset(uint32_t bit_position) {
    auto bit = 1U << bit_position;
    bits &= ~bit;
  }

  inline void reset() { bits = 0; }

  inline void flip(uint32_t bit_position) {
    auto bit = 1U << bit_position;
    bits ^= bit;
  }

  inline bool test(uint32_t bit_position) {
    return (bits >> bit_position) & 1U; // ? true : false;
  }

  uint32_t operator[](uint32_t bit_position) {
    return (bits >> bit_position) & 1U;
  }

private:
  uint32_t bits = 0;
  uint32_t size = 0;
};
} // namespace pie