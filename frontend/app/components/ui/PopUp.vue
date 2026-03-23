<script setup lang="ts">
import {computed} from 'vue'

const {modelValue = false, title, type} = defineProps<{
  modelValue?: boolean,
  title: string,
  type: 'info' | 'success' | 'warning' | 'error',
}>()


const emit = defineEmits(['update:modelValue'])

function close() {
  emit('update:modelValue', false)
}

const TYPE_CONFIG = {
  info: {icon: 'ℹ️'},
  success: {icon: '✅'},
  warning: {icon: '⚠️'},
  error: {icon: '❌'},
}

const typeConfig = computed(() => TYPE_CONFIG[type])
</script>


<template>
  <Teleport to="body">
    <Transition name="popup">
      <div v-if="modelValue" class="popup-overlay" @click.self="close">
        <div class="popup-card">
          <div class="popup-header" :class="`popup-header--${type}`">
            <span class="popup-icon">{{ typeConfig.icon }}</span>
            <h2 class="popup-title">{{ title }}</h2>
          </div>

          <div class="popup-body">
            <slot/>
          </div>

          <button class="popup-btn" :class="`popup-btn--${type}`" @click="close">ОК</button>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>


<style scoped>

.popup-overlay {
  position: fixed;
  inset: 0;
  background: rgba(120, 60, 10, 0.35);
  backdrop-filter: blur(3px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  font-family: var(--font-description), sans-serif;
}

.popup-card {
  background: #fffaf3;
  border-radius: 18px;
  box-shadow: 0 8px 40px rgba(180, 90, 20, 0.18);
  width: 100%;
  max-width: 420px;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

.popup-header {
  font-family: var(--font-heading), serif;
  padding: 20px 28px 18px;
  display: flex;
  align-items: center;
  gap: 12px;
}

.popup-header--info {
  background: linear-gradient(135deg, #e8873a 0%, #d4622a 100%);
}

.popup-header--success {
  background: linear-gradient(135deg, #4caf7d 0%, #2e8b57 100%);
}

.popup-header--warning {
  background: linear-gradient(135deg, #f5c842 0%, #e0a800 100%);
}

.popup-header--error {
  background: linear-gradient(135deg, #e85c3a 0%, #c0392b 100%);
}

.popup-wheat {
  font-size: 22px;
  opacity: 0.85;
  filter: grayscale(0.2);
}

.popup-icon {
  font-size: 22px;
  line-height: 1;
}

.popup-title {
  margin: 0;
  color: #fff;
  font-size: 1.15rem;
  letter-spacing: 0.04em;
  text-shadow: 0 1px 4px rgba(0, 0, 0, 0.12);
}

.popup-body {
  padding: 24px 28px 20px;
  color: var(--text);
  font-size: 0.97rem;
  line-height: 1.65;
}

.popup-btn {
  margin: 0 28px 24px;
  padding: 13px 0;
  border: none;
  border-radius: 10px;
  font-family: var(--font-description), sans-serif;
  font-size: 0.9rem;
  font-weight: 700;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  color: #fff;
  cursor: pointer;
  transition: opacity 0.15s, transform 0.1s;
}

.popup-btn--info {
  background: linear-gradient(135deg, #e8873a, #d4622a);
}

.popup-btn--success {
  background: linear-gradient(135deg, #4caf7d, #2e8b57);
}

.popup-btn--warning {
  background: linear-gradient(135deg, #f5c842, #e0a800);
  color: #5a3e00;
}

.popup-btn--error {
  background: linear-gradient(135deg, #e85c3a, #c0392b);
}

.popup-btn:hover {
  opacity: 0.88;
  transform: translateY(-1px);
}

.popup-btn:active {
  opacity: 1;
  transform: translateY(0);
}

/* ── Transition ── */
.popup-enter-active,
.popup-leave-active {
  transition: opacity 0.2s ease;
}

.popup-enter-active .popup-card,
.popup-leave-active .popup-card {
  transition: transform 0.2s ease, opacity 0.2s ease;
}

.popup-enter-from,
.popup-leave-to {
  opacity: 0;
}

.popup-enter-from .popup-card,
.popup-leave-to .popup-card {
  transform: scale(0.95) translateY(8px);
  opacity: 0;
}
</style>