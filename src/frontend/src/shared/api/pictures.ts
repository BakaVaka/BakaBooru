import axios from 'axios'

export const BASE_URL = ''

const apiClient = axios.create({
  baseURL: BASE_URL,
  headers: {
    'Content-Type': 'application/json',
    'Access-Control-Allow-Origin': '*',
    'Access-Control-Allow-Headers': '*',
    'Access-Control-Allow-Methods': '*'
  }
})

export const getPictures = async (page = 1, size = 10, tags = '') => {
  try {
    const response = await apiClient.get('/api/Pictures', {
      params: {
        page,
        size,
        tags
      }
    })
    console.log(response.data)
    return response.data
  } catch (error) {
    console.error('Error fetching pictures: ', error)
  }
}

export const uploadPicture = async (name: string, description: string, file: any, tags: string) => {
  try {
    const formData = new FormData()
    formData.append('Name', name)
    formData.append('Description', description)
    formData.append('File', file)
    formData.append('Tags', tags)

    const response = await apiClient.post('/api/Pictures', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
    return response.data
  } catch (error) {
    console.error('Error uploading picture:', error)
  }
}
