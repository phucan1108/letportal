import { Injectable } from '@angular/core';
import { App } from './portal.service';

/**
 * This service is centralized all session info. It is preventing to access directly session storage
 * For ex: get current user info, get current app info
 */
@Injectable({
        providedIn: 'root'
})
export class SessionService{

    setCurrentApp(app: App){
        sessionStorage.setItem('portal-app', JSON.stringify(app));
    }

    getCurrentApp(): App{
        if(sessionStorage.getItem('portal-app')){
            return JSON.parse(sessionStorage.getItem('portal-app'));
        }
        return null;
    }

    getDefaultAppPage(){
        return this.getCurrentApp().defaultUrl
    }

    setUserSession(userSessionId: string){
        sessionStorage.setItem('user-session-id', userSessionId)
    }

    getUserSession(){
        if(sessionStorage.getItem('user-session-id')){
            return JSON.parse(sessionStorage.getItem('user-session-id'));
        }
        return null;
    }

    setUserToken(jwtToken: string){
        sessionStorage.setItem('user-token', jwtToken)
    }

    getUserToken(){
        if(sessionStorage.getItem('user-token')){
            return JSON.parse(sessionStorage.getItem('user-token'));
        }
        return null;
    }

    clear(){
        sessionStorage.clear()
    }
}