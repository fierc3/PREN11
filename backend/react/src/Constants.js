// Constants.js
const prod = {
    url: {
     API_URL: 'https://tactile-rigging-333212.oa.r.appspot.com:6911'
    }
   };
   const dev = {
    url: {
     API_URL: 'http://localhost:6911'
    }
   };
   export const config = process.env.NODE_ENV === 'development' ? dev : prod;