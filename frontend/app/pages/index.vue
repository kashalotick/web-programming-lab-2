<script setup lang="ts">

const config = useRuntimeConfig()

async function click() {
  const result = await $fetch('rooms', {
    baseURL: config.public.apiBase,
  })
  console.log(result)
}

</script>

<template>
  <button @click="click">Fetch</button>
</template>

<style scoped>

</style>