export const environment = {
  production: false,
  version: '0.8.0',
  configurationEndpoint: 'http://localhost:53595/v1.0/api/configurations/Portal/v1.0',
  ignoreSendTokenEndpoints: 'api/accounts/login;api/accounts/refresh;api/accounts/forgot-password;api/accounts/recovery-password',
  chatOptions: {
    allowFileTypes: 'jpg;jpeg;gif;png;zip;rar;doc;docx;xls;xlsx;pdf',
    maxFileSizeInMb: 16
  },
  localization:{
    defaultLanguage: 'en-Us',
    allowSwitchLanguage: true,
    allowedLanguages: [
      { name: 'English', value: 'en-Us' },
      { name: 'Tiếng Việt', value: 'vi-VN' }
    ]
  }
};
