export const environment = {
  production: true,
  version: '0.8.0',
  configurationEndpoint: 'https://192.168.20.8:8080/v1.0/api/configurations/Portal/v1.0',
  ignoreSendTokenEndpoints: 'api/accounts/login;api/accounts/refresh;api/accounts/forgot-password;api/accounts/recovery-password',
  chatOptions: {
    allowFileTypes: 'jpg;jpeg;gif;png;zip;rar;doc;docx;xls;xlsx;pdf',
    maxFileSizeInMb: 16
  }
};
