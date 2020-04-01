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
    sessionId: string
    chatRoomId: string
    previousSessionId: string
    nextSessionId: string
    messages: Message[]
}

export interface Message{
    message: string
    formattedMessage: string
    fileUrls: string[]
    userName: string
    timeStamp: number
    createdDate: Date
}

export interface ExtendedMessage extends Message{
    isReceived: boolean
    chatSessionId: string
}