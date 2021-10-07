#pragma once
#include "bitset.hpp"
#include <stddef.h>
#include <stdint.h>
namespace pie {

template <typename T, uint8_t SIZE> class Vector {
public:
  Vector();
  class Iterator;
  typedef T value_type;
  typedef T &reference;
  typedef const T &const_reference;
  typedef T *pointer;
  // typedef T *iterator;
  // typedef const T *const_iterator;
  typedef uint32_t size_type;

  Iterator begin() { return Iterator(this); }

  // const_iterator begin() const { return elements; }

  Iterator end() { return Iterator(this, true); }
  // const_iterator end() const { return elements + SIZE; }
  size_type size() const { return SIZE - free_elements_count; }
  size_type full() const { return !free_elements_count; }
  void push_back(pointer value);

private:
  pointer elements[SIZE];       /*!< Holds the vector pointer objects. */
  uint32_t free_elements_count; /*!< Counts number of free elements */
  /*!< Keeps track of free/allocated memory objects. */
  pie::bitset_32 info{SIZE};
  uint8_t indexes[SIZE + 1]; /* index of used vector elements, last element is
                                set to -1 */
  uint8_t end_pos{0};
};

template <typename T, uint8_t SIZE> Vector<T, SIZE>::Vector() {
  while (32 < SIZE) {
    // info can be maximal of 32 bits
  }
  info.set();
  free_elements_count = SIZE;
}

template <typename T, uint8_t SIZE>
void Vector<T, SIZE>::push_back(pointer value) {
  for (auto i = 0U; i < SIZE; ++i) {
    if (info.test(i)) {
      elements[i] = value;
      info.reset(i);
      --free_elements_count;
      break;
    }
  }
  uint8_t i = 0;
  auto index = 0;
  for (; i < SIZE; ++i) {
    if (!info.test(i)) {
      indexes[index] = i;
      ++index;
    }
  }
  indexes[index] = -1;
  end_pos = index;
}

template <typename T, uint8_t SIZE> class Vector<T, SIZE>::Iterator {
  // typedef typename template <typename T, uint32_t SIZE> Vector<T, SIZE>
  // Vector;

public:
  Iterator(Vector<T, SIZE> *vector, bool is_end_position = false)
      : vector{vector} {
    // uint32_t pos = 0;
    // for (auto i = 0U; i < SIZE; ++i) {
    //   if (!vector->info.test(i)) {
    //     indexes[pos] = i;
    //     ++pos;
    //   }
    // }

    // indexes[pos] = -1U;
    is_end_position ? current_pos = vector->end_pos : current_pos = 0;
  }

  Iterator operator++() {
    ++current_pos;
    return *this;
  }

  pointer operator*() const {
    auto pos = vector->indexes[current_pos];
    return pos != -1U ? vector->elements[pos] : vector->elements[SIZE];
  }

  bool operator!=(const Iterator &rhs) const {
    return this->current_pos != rhs.current_pos;
  }

private:
  Vector<T, SIZE> *vector;
  // uint32_t indexes[SIZE];
  uint32_t current_pos{0};
};

} // namespace pie