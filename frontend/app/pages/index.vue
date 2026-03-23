<script setup lang="ts">

</script>

<template>
  <div class="main">
    <NuxtLink to="/book">Гість</NuxtLink>
    <NuxtLink to="/admin/reservations">Бронювання</NuxtLink>

  </div>


</template>

<style scoped>
.main {
  display: flex;
  flex-direction: column;
  gap: 2rem;
  justify-content: center;
  align-items: center;
  width: 100%;
  height: 100dvh;
  font-size: 2rem;
}

</style>