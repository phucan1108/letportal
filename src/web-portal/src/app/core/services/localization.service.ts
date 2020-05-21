import { Injectable } from "@angular/core";
import { LocalizationContent } from './portal.service';
import { environment } from 'environments/environment';

@Injectable({
    providedIn: 'root'
})
export class LocalizationService{
    languageKeys: LocalizationContent[] = []
    allowTranslate = environment.localization.allowSwitchLanguage
    isLoaded = false
    setKeys(languages: LocalizationContent[]){
        this.languageKeys = languages
        this.isLoaded = true
    }

    setAllowTranslate(allow: boolean){
        this.allowTranslate = allow
    }

    getText(key: string){
        try
        {
            return this.languageKeys.find(a => a.key === key).text
        }
        catch
        {
            return ''
        }
    }
}