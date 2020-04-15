import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { ChatService } from 'services/chat.service';
import { SecurityService } from './core/security/security.service';
import { VideoCallService } from 'services/videocall.service';
import { Store, Actions, ofActionCompleted } from '@ngxs/store';
import { UserDroppedCall, DroppedCall } from 'stores/chats/chats.actions';
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
    });
  }
}
