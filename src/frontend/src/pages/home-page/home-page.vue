<script lang="ts" setup>
import { onMounted, ref } from 'vue'
import { BASE_URL, getPictures, uploadPicture } from '@/shared/api/pictures'

const page = ref(1)
const size = ref(10)
const tags = ref<string | null>(null)
const data = ref<any>({})
const links = ref<string[]>([])
const largeImg = ref<string | null>(null)
const showUploadForm = ref<boolean>(false)

onMounted(async () => {
  console.log('Mounted')
  await fetchData()
})

async function fetchData() {
  try {
    const tagsArray = tags.value?.split(' ')
    const apiTags = tagsArray?.join(',')
    const result = await getPictures(page.value, size.value, apiTags)
    data.value = result
    getPicturesLinks()
  } catch (e) {
    console.error(e)
  }
}

function getPicturesLinks() {
  const res = []
  for (let it of data.value.data) {
    const link = `${BASE_URL}${it.path}`
    res.push(link)
  }
  links.value = res
}
async function loadPage(n: number) {
  if (n < 1) return
  page.value = n
  await fetchData()
}

function showLarge(src: string | null) {
  largeImg.value = src
}

const uploadPictureName = ref<string>('')
const uploadPictureDescription = ref<string>('')
const uploadPictureTags = ref<string>('')
const uploadPictureFile = ref<any>(null)

async function onUploadImageSubmit() {
  console.log(uploadPictureFile)

  await uploadPicture(
    uploadPictureName.value,
    uploadPictureDescription.value,
    uploadPictureFile.value.files[0],
    uploadPictureTags.value
  )

  showUploadForm.value = false
  await fetchData()
}
</script>

<template>
  <div class="app-container">
    <div class="search-container">
      <input v-model="tags" placeholder="Введите теги" />
      <button @click="fetchData">Искать</button>
      <button @click="showUploadForm = true">Загрузить</button>
    </div>
    <h1>Картинки</h1>
    <div class="app-content" v-if="data.data">
      <div class="imgs-container">
        <img class="picture" v-for="it in links" :key="it" :src="it" @click="showLarge(it)" />
      </div>
      <div class="nav-menu">
        <button :disabled="data.page == 1" @click="loadPage(page - 1)">Туда</button>
        <div>{{ data.page }} / {{ data.lastPage }}</div>
        <button :disabled="!(data.page < data.lastPage)" @click="loadPage(page + 1)">Сюда</button>
      </div>
    </div>
    <div v-if="largeImg" class="large-img-container" @click="showLarge(null)">
      <img :src="largeImg" />
    </div>
    <div v-if="showUploadForm" id="form-upload-container">
      <form id="uploadForm" @submit.prevent="onUploadImageSubmit">
        <h2>Загрузка картинки</h2>
        <input v-model="uploadPictureName" id="name" type="text" placeholder="Имя" />
        <textarea v-model="uploadPictureDescription" id="description" placeholder="Описание" />
        <input ref="uploadPictureFile" id="file" type="file" />
        <textarea v-model="uploadPictureTags" id="tags" placeholder="Теги через запятую" />
        <input @submit.prevent="onUploadImageSubmit" type="submit" value="Загрузить картинку" />
      </form>
    </div>
  </div>
</template>

<style>
.app-container {
  margin: 10px;
  padding: 24px;
  display: flex;
  flex-direction: column;
  /* height: 100vh; */
}
.search-container {
  display: flex;
  gap: 5px;

  input {
    width: 100%;
    padding: 10px;
  }
}
.nav-menu {
  margin: 10px;
  padding: 10px;
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  button {
    padding: 10px;
    font-size: 12pt;
  }
}
.imgs-container {
  margin: 10px;
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
  gap: 32px;
}
.picture {
  max-width: 256px;
  max-height: 256px;
  box-shadow: 0 0 12px #aaaaaa;
  cursor: pointer;
}

.app-content {
  /* height: 100%; */
  display: flex;
  flex-direction: column;
  justify-content: space-between;
}
.large-img-container {
  position: absolute;
  left: 0;
  top: 0;
  width: 100vw;
  height: 100vh;
  background: #00000086;
  display: flex;
  justify-content: center;
  img {
    margin: auto;
    position: relative;
    max-height: 100vh;
  }
}
#form-upload-container {
  z-index: 0;
  position: absolute;
  top: 0;
  left: 0;
  height: 100vh;
  width: 100vw;
  display: flex;
  justify-content: center;
  flex-direction: column;
  background: #00000086;
}
#uploadForm {
  z-index: 9999;
  border-radius: 16px;
  margin: auto;
  width: 640px;
  padding: 10px;
  background-color: #ffffff;
  display: flex;
  flex-direction: column;
  gap: 10px;
  input,
  textarea {
    padding: 10px;
  }
}
</style>
