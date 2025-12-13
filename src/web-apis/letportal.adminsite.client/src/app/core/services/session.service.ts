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
        return this.getCurrentApp() ? this.getCurrentApp().defaultUrl : '/dashboard'
    }

    clearSelectedApp(){
        sessionStorage.removeItem('portal-app')
    }
    setUserSession(userSessionId: string){
        sessionStorage.setItem('user-session-id', userSessionId)
    }

    getUserSession(){
        if(sessionStorage.getItem('user-session-id')){
            return sessionStorage.getItem('user-session-id');
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

    setRememberMe(rememberMe: boolean){
        localStorage.setItem('rememberMe', rememberMe.toString())
    }

    getRememberMe(){
        const rememberMe = localStorage.getItem('rememberMe')
        if(rememberMe){
            return rememberMe.toLowerCase() == 'true'
        }
        return false
    }

    clearUserToken(){
        sessionStorage.removeItem('user-token')
    }

    clearUserSession(){
        sessionStorage.removeItem('user-session-id')
    }

    clear(){
        sessionStorage.clear()
    }
}