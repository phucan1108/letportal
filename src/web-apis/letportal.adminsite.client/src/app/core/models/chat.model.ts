import { OnlineUser } from 'services/chat.service';

export interface ChatOnlineUser extends OnlineUser{
    incomingMessages: number
}

export interface ChatRoom{
    chatRoomId: string
    roomName: string
    type: RoomType
    participants: ChatOnlineUser[]
    chatSessions: ChatSession[]
    currentSession: ChatSession
    lastVisited: number 
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
    messages: ExtendedMessage[]
    previousSessionId: string
    nextSessionId: string
}

export interface Message{
    message: string
    formattedMessage: string
    attachmentFiles: AttachmentFile[]
    userName: string
    timeStamp: number
    createdDate: Date
}

export interface ExtendedMessage extends Message{
    isReceived: boolean
    hasAttachmentFile: boolean
    chatSessionId: string
    renderTime: boolean
}

export interface AttachmentFile{
    downloadUrl: string
    fileType: string
    fileName: string
}

export interface VideoRoomModel{
    id: string
    isConnectedRtc: boolean
    participants: ParticipantVideo[]
    handshakeDate: Date
    droppedDate: Date
}

export interface ParticipantVideo{
    connectionId: string
    username: string
}

export interface RtcIceServer{
    urls: string
    username: string
    credential: string
}

export interface VideoRtcSignal{
    roomId: string
    signalMessage: string
    connectionId: string
}