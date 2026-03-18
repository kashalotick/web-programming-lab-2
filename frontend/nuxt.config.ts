export default defineNuxtConfig({
  ssr: false,
  css: ['@/assets/css/main.css'],

  app:{
    head: {
      title: 'Web programming lab 2',
      link: [
        { rel: 'preconnect', href: 'https://fonts.googleapis.com' },
        { rel: 'preconnect', href: 'https://fonts.gstatic.com', crossorigin: '' },
        { rel: 'stylesheet', href: 'https://fonts.googleapis.com/css2?family=Kurale&display=swap' }
      ]
    }
  },
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },

  runtimeConfig: {
    public: {
      apiBase: process.env.API_URL,
    },
  },

  modules: ['@nuxt/icon']
})