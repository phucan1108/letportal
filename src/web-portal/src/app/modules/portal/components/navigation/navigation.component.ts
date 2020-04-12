import { Component, OnInit, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Observable } from 'rxjs';
import { map, filter, tap } from 'rxjs/operators';
import { Store } from '@ngxs/store';
import { UserSelectAppAction } from 'stores/apps/app.action';
import { Menu } from 'services/portal.service';
import { Router, Event, ActivatedRoute, NavigationStart, NavigationEnd, NavigationCancel, NavigationError } from '@angular/router';
import { SessionService } from 'services/session.service';
import { SecurityService } from 'app/core/security/security.service';
import { AuthUser } from 'app/core/security/auth.model';
import { AppDashboardComponent } from '../app-dashboard/app-dashboard.component';
import { PortalStandardClaims } from 'app/core/security/portalClaims';
import { ObjectUtils } from 'app/core/utils/object-util';
import { NGXLogger } from 'ngx-logger';
import { ChatService } from 'services/chat.service';
import { VideoCallService } from 'services/videocall.service';

@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class NavigationComponent implements OnInit {

  authUser: AuthUser

  loading = false
  menus: Array<Menu> = [];
  isExpanded = true;
  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(
      map(result => result.matches)
    );

  hasMenu: boolean = !!this.menus && this.menus.length > 0
  hasSelectedApp = false
  hasAvatar = false
  userShortName = ''
  constructor(
    private logger: NGXLogger,
    private breakpointObserver: BreakpointObserver,
    private store: Store,
    private activatedRouter: ActivatedRoute,
    private router: Router,
    private session: SessionService,
    private security: SecurityService,
    private chatService: ChatService,
    private videoService: VideoCallService,
    private cd: ChangeDetectorRef) {
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
    this.router.events.subscribe((event: Event) => {
      switch (true) {
        case event instanceof NavigationStart:
          this.loading = true
          break
        case event instanceof NavigationEnd:
        case event instanceof NavigationCancel:
        case event instanceof NavigationError:
          this.loading = false
          break
        default:
          break
      }
    })
  }

  ngOnInit(): void {
    this.hasSelectedApp = this.session.getCurrentApp() ? true : false
    this.authUser = this.security.getAuthUser()
    this.hasAvatar = ObjectUtils.isNotNull(this.authUser.avatar)

    this.userShortName = this.authUser.getShortName()
    // If user stays in App Selector page, we don't display menu
    if (this.activatedRouter.snapshot.firstChild.component !== AppDashboardComponent) {
      this.store.select(a => a.selectedApp).pipe(
        filter(result => result.filterState === UserSelectAppAction),
        tap(result => {
          this.hasSelectedApp = true
          this.menus = this.removeMenusByClaims(ObjectUtils.clone(result.selectedApp.menus), this.security.getAuthUser())
          this.cd.markForCheck()
        })
      ).subscribe()
      const currentApp = this.session.getCurrentApp();
      if (currentApp) {
        this.menus = this.removeMenusByClaims(ObjectUtils.clone(currentApp.menus), this.security.getAuthUser())
      }
    }
  }

  logout() {  
    this.security.userLogout()
    this.session.clear()
    this.chatService.stop()
    this.videoService.stop() 
    // Due to prevent angular cache and store
    // We SHOULD force reload a page
    // Thus, we will get a performance because user will reload a page   
    window.location.href = '/'
  }

  profile(){
    this.router.navigateByUrl('portal/page/user-info')
  }

  appDashboard(){
    this.session.clearSelectedApp()
    this.router.navigateByUrl('portal/dashboard')
  }

  private removeMenusByClaims(menus: Menu[], user: AuthUser) {
    if(!user.isInRole('SuperAdmin')){
      menus.forEach(menu => {
        if (!menu.hide && menu.subMenus) {
          let count = menu.subMenus.length
          menu.subMenus.forEach(sub => {
            if (!sub.hide) {
              const startIndex = sub.url.indexOf('portal/page/')
              if (startIndex > 0) {
                const startCutIndex = startIndex + 12
                const endCutIndex = sub.url.lastIndexOf('?') > 0 ? sub.url.lastIndexOf('?') : sub.url.length
                const pageName = sub.url.substr(startCutIndex, endCutIndex - startCutIndex)
                const isAllow = user.hasClaim(pageName, PortalStandardClaims.AllowAccess.name)
                this.logger.debug('User has claim of page ' + pageName, isAllow)
                if (!isAllow) {
                  count--
                  sub.hide = true
                }
              }
            }
          })
          if(count == 0){
            menu.hide = true
          }
        }
      })

      return menus.filter(m => !m.hide)
    }
    else{
      return menus
    }
  }
}
