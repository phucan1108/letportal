import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import 'hammerjs';
import { AppModule } from './app/app.module';
import { environment } from './environments/environment';


if (environment.production) {
  enableProdMode();
}

// Workround solution for APP_INITIALIZER don't run before any angular bootstrap
// Prefer to https://github.com/angular/angular/issues/23279 issue
fetch(environment.configurationEndpoint, {
  method: 'GET',
  headers: {
    'X-User-Session-Id': btoa('NEW_TAB_' + window.navigator.platform),
    'Content-Type': 'application/json'
  }
})
  .then(result => result.text())
  .then(configs => {
    if (configs) {
      window.sessionStorage.setItem('configurations', configs)
      platformBrowserDynamic().bootstrapModule(AppModule)
        .catch(err => console.error(err));
    }
    else {
      document.write('There are some errors, please press F5 to reload the page.')
    }
  })
  .catch((err) => {
    document.write('There are some errors, please press F5 to reload the page.')
    document.write(`Technical error: ${err}`)
  })

