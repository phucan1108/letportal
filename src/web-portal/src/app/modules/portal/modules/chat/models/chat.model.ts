import { OnlineUser } from 'services/chat.service';

export interface ChatOnlineUser extends OnlineUser{
    
}

export interface ChatRoom{
    chatRoomId: string
    roomName: string
    type: RoomType
    participants: ChatOnlineUser[]
    chatSessions: ChatSession[]
    currentSession: ChatSession    
}

export enum RoomType{
    Double,
    Group
}

export interface DoubleChatRoom extends ChatRoom{
    invitee: ChatOnlineUser
}

export interface ChatSession{
    chatSessionId: string
    preSessionId: string
    nextSessionId: string
    messages: Message[]
}

export interface Message{
    message: string
    userName: string
}