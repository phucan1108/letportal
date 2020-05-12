import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router, NavigationEnd, NavigationStart } from '@angular/router';
import { ChatService } from 'services/chat.service';
import { SecurityService } from './core/security/security.service';
import { VideoCallService } from 'services/videocall.service';
import { Store, Actions, ofActionCompleted } from '@ngxs/store';
import { UserDroppedCall, DroppedCall } from 'stores/chats/chats.actions';
import { TranslateService } from '@ngx-translate/core';
import { ObjectUtils } from './core/utils/object-util';
import { environment } from 'environments/environment';
import * as moment from 'moment'
import { LocalizationService } from 'services/localization.service';
import { LocalizationClient } from 'services/portal.service';
import { tap } from 'rxjs/operators';
import { SessionService } from 'services/session.service';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  showChatBox = false
  showVideoBox = true
  constructor(
    private router: Router,
    private chatService: ChatService,
    private videoService: VideoCallService,
    private securityService: SecurityService,
    private translate: TranslateService,
    private localizationService: LocalizationService,
    private session: SessionService,
    private localizationsClient: LocalizationClient
  ) {
  }
  ngOnInit(): void {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        if (event.urlAfterRedirects.indexOf('/portal/') >= 0
          && this.securityService.isUserSignedIn()) {
          this.chatService.start()
          this.chatService.online()
          this.videoService.start()
          setTimeout(() => {
            this.chatService.getAllAvailableUsers()
            this.showChatBox = true
            this.showVideoBox = true
          }, 200)
        }
        else {
          this.showChatBox = false
        }
      }
      
      if(event instanceof NavigationStart){
        if(event.url.indexOf('/portal/') >= 0){
          if (this.localizationService.allowTranslate
            && this.translate.currentLang !== environment.localization.defaultLanguage
            && !this.localizationService.isLoaded) {
            const currentLang = this.translate.currentLang
            const currentApp = this.session.getCurrentApp()
            if(ObjectUtils.isNotNull(currentApp)){
              this.localizationsClient.getOne(currentApp.id, currentLang).pipe(
                tap(
                  keys => {
                    this.localizationService.setKeys(keys.localizationContents)
                  }
                )
              ).subscribe()
            }
          }
        }
      }
    });

    if (ObjectUtils.isNotNull(localStorage.getItem('lang'))) {
      this.translate.use(localStorage.getItem('lang'))
      moment.locale(localStorage.getItem('lang'))
    }
    else {
      localStorage.setItem('lang', environment.localization.defaultLanguage)
      moment.locale(environment.localization.defaultLanguage)
    }
  }
}
