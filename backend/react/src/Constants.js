// Constants.js
const prod = {
    url: {
     API_URL: 'https://tactile-rigging-333212.oa.r.appspot.com'
    }
   };
   const dev = {
    url: {
     API_URL: 'http://localhost:3001'
    }
   };
   export const config = process.env.NODE_ENV === 'development' ? dev : prod;