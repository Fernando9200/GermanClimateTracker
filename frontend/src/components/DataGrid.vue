<template>
  <div class="container">
    <el-card class="box-card">
      <template #header>
        <div class="card-header">
          <div class="header-controls">
            <el-input
              v-model="searchQuery"
              placeholder="Search cities..."
              class="search-input"
              :prefix-icon="Search"
            />
            <el-button-group class="view-toggle">
              <el-button
                :class="{ 'selected-button': viewMode === 'table' }"
                @click="viewMode = 'table'"
              >
                <el-icon :style="{ color: viewMode === 'table' ? '#409EFF' : '' }"><Menu /></el-icon>
              </el-button>
              <el-button
                :class="{ 'selected-button': viewMode === 'cards' }"
                @click="viewMode = 'cards'"
              >
                <el-icon :style="{ color: viewMode === 'cards' ? '#409EFF' : '' }"><Grid /></el-icon>
              </el-button>
            </el-button-group>
          </div>
        </div>
      </template>
      <!-- Table View -->
      <el-table
        v-if="viewMode === 'table'"
        :data="filteredData"
        :border="true"
        :stripe="true"
        v-loading="loading"
        class="weather-table"
        @sort-change="handleSort"
      >
        <el-table-column
          prop="city"
          label="City"
          sortable="custom"
          min-width="120"
        >
          <template #default="{ row }">
            <div class="city-cell">
              <span class="city-name">{{ row.city }}</span>
            </div>
          </template>
        </el-table-column>

        <el-table-column
          prop="temperature"
          label="Temperature"
          sortable="custom"
          min-width="150"
        >
          <template #default="{ row }">
            <div class="temp-cell">
              <img
                :src="`https://openweathermap.org/img/wn/${row.icon}@2x.png`"
                :alt="row.description"
                class="weather-icon"
              />
              <span class="temperature">{{ formatTemperature(row.temperature) }}</span>
            </div>
          </template>
        </el-table-column>

        <el-table-column
          prop="pressure"
          label="Pressure"
          sortable="custom"
          min-width="120"
        >
          <template #default="{ row }">
            <div class="pressure-cell">
              <el-icon><Odometer /></el-icon>
              {{ row.pressure }} hPa
            </div>
          </template>
        </el-table-column>

        <el-table-column
          prop="humidity"
          label="Humidity"
          sortable="custom"
          min-width="120"
        >
          <template #default="{ row }">
            <div class="humidity-cell">
              <el-icon><Umbrella /></el-icon>
              {{ row.humidity }}%
            </div>
          </template>
        </el-table-column>

        <el-table-column
          prop="windSpeed"
          label="Wind Speed"
          sortable="custom"
          min-width="120"
        >
          <template #default="{ row }">
            <div class="wind-cell">
              <el-icon><WindPower /></el-icon>
              {{ row.windSpeed }} m/s
            </div>
          </template>
        </el-table-column>

        <el-table-column
          prop="description"
          label="Conditions"
          min-width="150"
        >
          <template #default="{ row }">
            <div class="description-cell">
              {{ capitalizeFirst(row.description) }}
            </div>
          </template>
        </el-table-column>

        <el-table-column
          prop="timestamp"
          label="Last Updated"
          sortable="custom"
          min-width="120"
        >
          <template #default="{ row }">
            <div class="timestamp-cell">
              <el-icon><Timer /></el-icon>
              {{ formatDate(row.timestamp) }}
            </div>
          </template>
        </el-table-column>
      </el-table>

      <!-- Card View -->
      <div v-else class="cards-view">
        <el-row :gutter="20">
          <el-col
            v-for="item in filteredData"
            :key="item.id"
            :xs="24"
            :sm="12"
            :md="8"
            :lg="6"
            class="card-col"
          >
            <el-card class="weather-card">
              <div class="weather-card-content">
                <h3 class="city-name">{{ item.city }}</h3>
                <div class="weather-icon-large">
                  <img
                    :src="`https://openweathermap.org/img/wn/${item.icon}@4x.png`"
                    :alt="item.description"
                  />
                </div>
                <div class="weather-details">
                  <p class="temperature-large">
                    {{ formatTemperature(item.temperature) }}
                  </p>
                  <p class="description">
                    {{ capitalizeFirst(item.description) }}
                  </p>
                  <div class="metrics">
                    <span>
                      <el-icon><Umbrella /></el-icon>
                      {{ item.humidity }}%
                    </span>
                    <span>
                      <el-icon><WindPower /></el-icon>
                      {{ item.windSpeed }} m/s
                    </span>
                  </div>
                  <p class="timestamp">
                    <el-icon><Timer /></el-icon>
                    {{ formatDate(item.timestamp) }}
                  </p>
                </div>
              </div>
            </el-card>
          </el-col>
        </el-row>
      </div>

      <div v-if="error" class="error-message">
        <el-alert
          :title="error"
          type="error"
          show-icon
        />
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { Search, Umbrella, WindPower, Timer, Odometer, Menu, Grid } from '@element-plus/icons-vue'
import axios from 'axios'

interface WeatherData {
  id: string;
  city: string;
  temperature: number;
  pressure: number;
  humidity: number;
  description: string;
  windSpeed: number;
  timestamp: string;
  icon: string;
}

const items = ref<WeatherData[]>([])
const searchQuery = ref('')
const loading = ref(false)
const error = ref('')
const viewMode = ref<'table' | 'cards'>('table')
const sortBy = ref('')
const sortOrder = ref<'ascending' | 'descending'>('ascending')

const filteredData = computed(() => {
  let result = [...items.value]

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(item =>
      item.city.toLowerCase().includes(query) ||
      item.description.toLowerCase().includes(query)
    )
  }

  if (sortBy.value) {
    result.sort((a, b) => {
      const aVal = a[sortBy.value as keyof WeatherData]
      const bVal = b[sortBy.value as keyof WeatherData]

      if (typeof aVal === 'string' && typeof bVal === 'string') {
        return sortOrder.value === 'ascending'
          ? aVal.localeCompare(bVal)
          : bVal.localeCompare(aVal)
      }

      return sortOrder.value === 'ascending'
        ? Number(aVal) - Number(bVal)
        : Number(bVal) - Number(aVal)
    })
  }

  return result
})

const handleSort = ({ prop, order }: { prop: string, order: string }) => {
  sortBy.value = prop
  sortOrder.value = order as 'ascending' | 'descending'
}

const formatTemperature = (celsius: number) => {
  return `${celsius.toFixed(1)}°C`
}

const capitalizeFirst = (str: string) => {
  return str.charAt(0).toUpperCase() + str.slice(1)
}

const formatDate = (timestamp: string) => {
  const utcTimestamp = timestamp.endsWith('Z') ? timestamp : timestamp + 'Z'
  const date = new Date(utcTimestamp)

  return date.toLocaleTimeString('de-DE', {
    hour: '2-digit',
    minute: '2-digit',
    hour12: false
  })
}

const fetchData = async () => {
  loading.value = true
  error.value = ''
  try {
    const response = await axios.get('http://localhost:5102/api/weather', {
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      },
      withCredentials: false
    })
    items.value = response.data
  } catch (e) {
    error.value = 'Error loading weather data. Please make sure the API is running.'
    console.error('Error fetching data:', e)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchData()
  setInterval(fetchData, 300000) // Refresh every 5 minutes
})
</script>

<style scoped>
.container {
  padding: 20px;
  max-width: 100%;
}

.box-card {
  border-radius: 8px;
}

.card-header {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.header-main {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-controls {
  display: flex;
  gap: 16px;
  align-items: center;
}

.title {
  margin: 0;
  font-size: 1.5rem;
  color: #2c3e50;
  white-space: nowrap;
}

.search-input {
  width: 300px;
  max-width: 100%;
}

.view-toggle {
  display: flex;
  gap: 8px;
}

.view-toggle .el-button {
  border: 1px solid #dcdfe6;
}

.view-toggle .el-button:hover {
  border-color: #c0c4cc;
  background-color: #f0f2f5;
}

.view-toggle .selected-button {
  border: 1px solid #409EFF;
  background-color: #ecf5ff;
}

.view-toggle .selected-button:hover {
  background-color: #ecf5ff;
  border: 1px solid #409EFF;
}

.view-toggle .el-button .el-icon {
  font-size: 1.2rem;
  transition: color 0.3s; /* For smooth color transitions */
}

.view-toggle .el-button:not(.selected-button) .el-icon {
  color: #606266;
}

.weather-table {
  width: 100%;
  table-layout: fixed;
}

.city-cell {
  font-weight: 600;
  color: #2c3e50;
}

.temp-cell {
  display: flex;
  align-items: center;
  gap: 8px;
}

.weather-icon {
  width: 40px;
  height: 40px;
  flex-shrink: 0;
}

.temperature {
  font-size: 1.1rem;
  font-weight: 500;
}

.humidity-cell,
.wind-cell,
.pressure-cell,
.timestamp-cell {
  display: flex;
  align-items: center;
  gap: 8px;
}

/* Card View Styles */
.cards-view {
  padding: 20px;
}

.card-col {
  margin-bottom: 20px;
}

.weather-card {
  height: 100%;
  transition: transform 0.2s;
}

.weather-card:hover {
  transform: translateY(-5px);
}

.weather-card-content {
  text-align: center;
}

.temperature-large {
  font-size: 2rem;
  font-weight: 600;
  margin: 16px 0;
}

.metrics {
  display: flex;
  justify-content: center;
  gap: 20px;
  margin: 16px 0;
}

.metrics span {
  display: flex;
  align-items: center;
  gap: 4px;
}

.el-icon {
  font-size: 1.2rem;
  color: #409EFF;
  flex-shrink: 0;
}

.error-message {
  margin-top: 16px;
}

:deep(.el-card__body) {
  padding: 0;
}

:deep(.el-table .cell) {
  padding: 8px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

:deep(.el-table) {
  border-radius: 0 0 8px 8px;
}

@media screen and (max-width: 768px) {
  .header-controls {
    flex-direction: column;
  }

  .search-input {
    width: 100%;
  }

  .title {
    text-align: center;
  }
}
</style>