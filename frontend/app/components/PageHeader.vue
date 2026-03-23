<script setup lang="ts">
const {heading, mode = null} = defineProps<{
  heading: string
  mode?: 'view' | 'edit' | 'create' | null
}>()


const modeClass = computed(() => {
  if (!mode) return ''
  return `mode-${mode}`
})
const modeText = computed(() => {
  if (!mode) return ''
  switch (mode) {
    case 'view':
      return 'Перегляд'
    case 'edit':
      return 'Редагування'
    case 'create':
      return 'Створення'
  }
  return ''
})

</script>

<template>

  <div class="page-header">
    <div>
      <div class="page-title">
        <h1>{{ heading }}
        </h1>
        <span v-if="modeClass" class="mode-badge" :class="modeClass">{{ modeText }}</span>

      </div>


      <div v-if="$slots.secondary" class="page-subtitle">
        <slot name="secondary"/>
      </div>
    </div>
    <div class="buttons">
      <slot name="buttons"/>
    </div>
  </div>


</template>

<style scoped>
.buttons {
  display: flex;
  gap: 8px;
}
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
  gap: 16px;
  flex-wrap: wrap;
}

.page-title {
  display: flex;
  align-items: center;
  gap: 12px;
}

.mode-badge {
  font-family: var(--font-description), sans-serif;
  font-size: .75rem;
  font-weight: 700;
  letter-spacing: .1em;
  text-transform: uppercase;
  padding: .28rem .75rem;
  border-radius: 20px;
}

.mode-view {
  background: var(--gray);
  color: #666;
}

.mode-edit {
  background: var(--background-dark);
  color: var(--accent-dark);
  border: 1px solid var(--accent-light);
}

.mode-create {
  background: #e3fbdf;
  color: var(--success);
  border: 1px solid var(--success);
}

.page-subtitle {
  font-family: var(--font-description), sans-serif;
  font-size: 0.875rem;
  color: #999;
  margin-top: 4px;
  letter-spacing: .04em;
}

</style>