import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { ChatService } from 'services/chat.service';
import { SecurityService } from './core/security/security.service';
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
    private securityService: SecurityService
  ){
  }
  ngOnInit(): void {  
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        if(event.urlAfterRedirects.indexOf('/portal/') >= 0
          && this.securityService.isUserSignedIn()){      
          this.showChatBox = true
          this.chatService.start()
          this.chatService.online()
        }
        else{
          this.showChatBox = false
        }
      }
    }); 
  }
}
