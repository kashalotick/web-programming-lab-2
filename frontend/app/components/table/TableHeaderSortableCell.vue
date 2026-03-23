<script setup lang="ts">
const {sortKey, sortBy, isDescending} = defineProps<{
  sortKey: string,
  sortBy: string | null,
  isDescending: boolean,
}>()

const emit = defineEmits<{
  'sort': [string],
}>()

function toggleSort() {
  emit('sort', sortKey)
}

const icon = computed(() => {
  if (sortBy !== sortKey) return "lucide:chevrons-up-down"
  return isDescending ? "lucide:chevron-down" : "lucide:chevron-up"
})
</script>

<template>
  <th @click="toggleSort" style="cursor: pointer; user-select: none;" :class="{'selected': (sortBy === sortKey)}">
    <div class="selectable">
      <slot></slot>
      <Icon :name="icon" size="1rem"></Icon>
    </div>
  </th>

</template>

<style scoped>
.selectable {
  display: flex;
  align-items: center;
  justify-content: space-between;
  transition: all .15s ease;
}
th {
  transition: all .15s ease;

}
th:hover {
  background: var(--background);
}
.selected {
  background: var(--accent-light);
  color: var(--text);
  &:hover {
    background: var(--accent);
  }
}

</style>