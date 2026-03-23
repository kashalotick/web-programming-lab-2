<script setup lang="ts">
const {sortBy, isDescending} = defineProps<{
  sortBy: null | 'check_in' | 'created_at' | 'grand_total',
  isDescending: boolean,
}>()

const emit = defineEmits<{
  'update:sortBy': ['check_in' | 'created_at' | 'grand_total' | null],
  'update:isDescending': [boolean],
}>()


function toggleSort(key: string) {
  if (key !in ['check_in', 'created_at', 'grand_total']) {
    console.warn(`Invalid sort key: ${key}`)
    return
  }
  if (key === sortBy) {
    if (isDescending) {
      emit('update:sortBy', null)
      emit('update:isDescending', false)
    } else {
      emit('update:isDescending', true)
    }
  } else {
    emit('update:sortBy', key as 'check_in' | 'created_at' | 'grand_total')
    emit('update:isDescending', false)
  }
}


</script>

<template>
  <thead>
  <tr>
    <th>ID</th>
    <th>Гість</th>
    <th>Кімната</th>
    <TableHeaderSortableCell
        sort-key="check_in"
        :sort-by
        :is-descending
        @sort="toggleSort"
    >Заїзд → Виїзд
    </TableHeaderSortableCell>
    <TableHeaderSortableCell
        sort-key="grand_total"
        :sort-by
        :is-descending
        @sort="toggleSort"
    >Сума
    </TableHeaderSortableCell>
    <th>Статус</th>
    <TableHeaderSortableCell
        sort-key="created_at"
        :sort-by
        :is-descending
        @sort="toggleSort"
    >Створено
    </TableHeaderSortableCell>
    <th>Дії</th>
  </tr>
  </thead>
</template>

<style scoped>
thead {
  background: var(--background-dark);
  border-bottom: 1.5px solid var(--accent-light);
}

th {
  font-family: var(--font-description), monospace;
  padding: .65rem 1rem;
  text-align: left;
  font-size: .62rem;
  font-weight: 700;
  letter-spacing: .1em;
  text-transform: uppercase;
  color: var(--accent-dark);
  white-space: nowrap;
}
</style>