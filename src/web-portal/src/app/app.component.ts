import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { ChatService } from 'services/chat.service';
import { SecurityService } from './core/security/security.service';
import { VideoCallService } from 'services/videocall.service';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  showChatBox = false
  constructor(
    private router: Router,
    private chatService: ChatService,
    private videoService: VideoCallService,
    private securityService: SecurityService
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
          this.chatService.getAllAvailableUsers()
          setTimeout(() => {
            this.showChatBox = true
          }, 500)
        }
        else {
          this.showChatBox = false
        }
      }
    });
  }
}
