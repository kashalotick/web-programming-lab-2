<script setup lang="ts">
const props = defineProps<{
  heading: string
  badges: string[]
}>()
</script>

<template>
  <slot name="secondary">

  </slot>
  <slot name="buttons">

  </slot>
</template>

<style scoped>

</style>