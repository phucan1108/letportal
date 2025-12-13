// Docker environment configuration

export const environment = {
  production: false,
  version: '0.9.0',
  configurationEndpoint: 'https://localhost:5102/api/configurations/Portal/v1.0',
  ignoreSendTokenEndpoints: 'api/accounts/login;api/accounts/refresh;api/accounts/forgot-password;api/accounts/recovery-password;assets/i18n',
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
