<script setup lang="ts">
import { RouterView } from 'vue-router'
import DataGrid from './components/DataGrid.vue'
</script>

<template>
  <div class="app-container">
    <DataGrid />
  </div>
</template>

<style scoped>
.app-container {
  padding: 20px;
  max-width: 1200px;
  margin: 0 auto;
}

h1 {
  text-align: center;
  margin-bottom: 20px;
}
</style>