<!-- components/CalendarDropdown.vue -->
<script setup lang="ts">
const {
  anchorTarget,
  isOpen,
  offset = 4,
} = defineProps<{
  anchorTarget: HTMLElement | null
  isOpen: boolean
  offset?: number
}>()

const emit = defineEmits<{
  'close': []
}>()

const containerRef = useTemplateRef('container')
const containerStyle = ref({ top: '0px', left: '0px' })

// Позиціонування під anchor елементом
function updatePosition() {
  if (!anchorTarget) return

  const rect = anchorTarget.getBoundingClientRect()
  const top = rect.bottom + window.scrollY + offset
  let left = rect.left + window.scrollX

  // Не виходимо за межі body справа
  const containerWidth = containerRef.value?.$el?.offsetWidth
      ?? containerRef.value?.offsetWidth
      ?? 420

  const bodyWidth = document.body.getBoundingClientRect().width
  if (left + containerWidth > bodyWidth) {
    left = bodyWidth - containerWidth - 10
  }

  containerStyle.value = { top: `${top}px`, left: `${left}px` }
}

watch(() => [isOpen, anchorTarget], ([open]) => {
  if (open) nextTick(updatePosition)
})

// Click outside
function handleClickOutside(event: MouseEvent) {
  const el = containerRef.value?.$el ?? containerRef.value
  const clickedInsideAnchor = anchorTarget?.contains(event.target as Node)
  const clickedInsideContainer = el?.contains?.(event.target as Node)

  if (!clickedInsideAnchor && !clickedInsideContainer) {
    emit('close')
  }
}

// Escape
function handleKeydown(event: KeyboardEvent) {
  if (event.key === 'Escape') emit('close')
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
  document.addEventListener('keydown', handleKeydown)
})
onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
  document.removeEventListener('keydown', handleKeydown)
})
</script>

<template>
  <Teleport v-if="anchorTarget && isOpen" to="body">
    <div
        ref="container"
        class="calendar-dropdown"
        :style="containerStyle"
        @mousedown.prevent
    >
      <slot />
    </div>
  </Teleport>
</template>

<style scoped>
.calendar-dropdown {
  position: absolute;
  z-index: 999;
  border-radius: 1rem;
  width: 420px;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
}
</style>