import { OnlineUser } from 'services/chat.service';

export interface ChatOnlineUser extends OnlineUser{
    shortName: string
    hasAvatar: boolean
}