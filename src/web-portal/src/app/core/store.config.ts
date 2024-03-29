import { NgxsDevtoolsOptions } from '@ngxs/devtools-plugin/src/symbols';
import { NgxsLoggerPluginOptions } from '@ngxs/logger-plugin/src/symbols';
import { NgxsConfig } from '@ngxs/store/src/symbols';
import { environment } from 'environments/environment';
import { ChatState } from 'stores/chats/chats.state';
import { NotificationState } from 'stores/notifications/notification.state';
import { SelectedAppState } from './stores/apps/app.state';
import { PageState } from './stores/pages/page.state';
import { PageBuilderState } from './stores/pages/pagebuilder.state';
import { PageControlEventState } from './stores/pages/pagecontrol.state';
import { ShellDataState } from './stores/shell/shell.state';

export const STATES_MODULES = [
  PageBuilderState,
  ShellDataState,
  SelectedAppState,
  PageState,
  PageControlEventState,
  ChatState,
  NotificationState];

export const OPTIONS_CONFIG: Partial<NgxsConfig> = {
  /**
   * Run in development mode. This will add additional debugging features:
   * - Object.freeze on the state and actions to guarantee immutability
   * todo: you need set production mode
   * import { environment } from '@env';
   * developmentMode: !environment.production
   */
  developmentMode: !environment.production
};

export const DEVTOOLS_REDUX_CONFIG: NgxsDevtoolsOptions = {
  /**
   * Whether the dev tools is enabled or note. Useful for setting during production.
   * todo: you need set production mode
   * import { environment } from '@env';
   * disabled: environment.production
   */
  disabled: environment.production
};

export const LOGGER_CONFIG: NgxsLoggerPluginOptions = {
  /**
   * Disable the logger. Useful for prod mode..
   * todo: you need set production mode
   * import { environment } from '@env';
   * disabled: environment.production
   */
  disabled: environment.production
};
