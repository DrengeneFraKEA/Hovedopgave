const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
    env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:7213';

const PROXY_CONFIG = [
  {
    context: [
      "/weatherforecast",
    ],
    target,
    secure: true
  },
  {
    "/adminrights": {
      "target": "http://localhost:7213",
      "secure": false,
      "changeOrigin": true
    }
  }
]

module.exports = PROXY_CONFIG;
