import { ChatRoom, RoomType, ChatOnlineUser, DoubleChatRoom, ChatSession, ExtendedMessage, ParticipantVideo, VideoRoomModel, RtcIceServer } from 'app/core/models/chat.model';
import { State, StateToken, Action, StateContext, Actions } from '@ngxs/store';
import * as ChatActions from './chats.actions'
import { ObjectUtils } from 'app/core/utils/object-util';
import { patch, updateItem, insertItem, removeItem } from '@ngxs/store/operators';
import { ArrayUtils } from 'app/core/utils/array-util';
import { ErrorCode } from 'services/identity.service';
import { Injectable } from '@angular/core';

const MAX_ROOMS = 4
export const CHAT_STATE_TOKEN = new StateToken<ChatStateModel>('chats');
export interface ChatStateModel {
    // For maintaining online users
    availableUsers: ChatOnlineUser[]

    // For chatting
    chatRooms: ChatRoom[]
    activeChatSession: ChatSession
    notifiedChatRooms: string[] // Contains chatRoomId has new message
    invitingUser: ChatOnlineUser
    currentUser: ChatOnlineUser
    lastSentHashCode: string
    isOpenChatBox: boolean

    // For Video call
    incomingVideoCall: ParticipantVideo
    inviterVideoCall: ChatOnlineUser
    handshakedVideoCall: VideoRoomModel
    iceServers: RtcIceServer[]
    callErrorCode: ErrorCode
}

@State<ChatStateModel>({
    name: CHAT_STATE_TOKEN,
    defaults: {
        availableUsers: [],
        chatRooms: [],
        activeChatSession: null,
        invitingUser: null,
        notifiedChatRooms: [],
        currentUser: null,
        lastSentHashCode: null,
        isOpenChatBox: false,
        incomingVideoCall: null,
        handshakedVideoCall: null,
        inviterVideoCall: null,
        iceServers: [],
        callErrorCode: null
    }
})
@Injectable()
export class ChatState {

    @Action(ChatActions.TakeUserOnline)
    public takeUserOnline(ctx: StateContext<ChatStateModel>, { event }: ChatActions.TakeUserOnline) {
        return ctx.setState(
            patch({
                currentUser: event.user
            })
        )
    }

    @Action(ChatActions.ToggleOpenChatRoom)
    public toggleOpenChatRoom(ctx: StateContext<ChatStateModel>, { toggle }: ChatActions.ToggleOpenChatRoom) {
        const state = ctx.getState()
        return ctx.setState(
            patch({
                isOpenChatBox: toggle
            })
        )
    }

    @Action(ChatActions.ClickedOnChatUser)
    public clickedOnChatUser(ctx: StateContext<ChatStateModel>, { event }: ChatActions.ClickedOnChatUser) {
        const state = ctx.getState()
        const foundRoom = state.chatRooms.find(a => a.type === RoomType.Double && a.participants.some(b => b.userName === event.inviee.userName))

        let foundUser = state.availableUsers.find(a => a.userName === event.inviee.userName)
        foundUser = {
            ...foundUser,
            incomingMessages: 0
        }

        ctx.setState(
            patch({
                availableUsers: updateItem<ChatOnlineUser>(a => a.userName === foundUser.userName, foundUser)
            })
        )
        if (ObjectUtils.isNotNull(foundRoom)) {
            return ctx.dispatch(new ChatActions.ActiveDoubleChatRoom({
                chatSession: foundRoom.currentSession
            }))
        }
        else {
            // No found room, mean need to fetch from server
            ctx.setState(
                patch({
                    invitingUser: event.inviee
                })
            )
            // Notify Chat Service to init double chat room
            return ctx.dispatch(new ChatActions.FetchDoubleChatRoom())
        }
    }

    @Action(ChatActions.InitialReceivedChatRoom)
    public initialReceivedChatRoom(ctx: StateContext<ChatStateModel>, { event }: ChatActions.InitialReceivedChatRoom) {
        const state = ctx.getState()
        ctx.setState(
            patch({
                invitingUser: event.inviee
            })
        )
        // Notify Chat Service to init double chat room
        return ctx.dispatch(new ChatActions.FetchDoubleChatRoom())
    }

    @Action(ChatActions.ClickedOnChatBox)
    public clickedOnChatBoxIcon(ctx: StateContext<ChatStateModel>, { event }: ChatActions.ClickedOnChatBox) {
        const state = ctx.getState()
        const foundRoom = state.chatRooms.find(a => a.chatRoomId === event.chatRoomId)
        if (ObjectUtils.isNotNull(foundRoom)) {
            // Clear all messages
            let notifiedChatRooms: string[] = ObjectUtils.clone(state.notifiedChatRooms)
            if (ObjectUtils.isNotNull(notifiedChatRooms)) {
                notifiedChatRooms = notifiedChatRooms.filter(a => a !== foundRoom.chatRoomId)

                ctx.setState(
                    patch({
                        notifiedChatRooms: notifiedChatRooms
                    })
                )
            }
            return ctx.dispatch(new ChatActions.ActiveDoubleChatRoom({
                chatSession: foundRoom.currentSession
            }))
        }
    }

    @Action(ChatActions.ActiveChatSearchBox)
    public activeChatBox(ctx: StateContext<ChatStateModel>, { }: ChatActions.ActiveChatSearchBox) {
        const state = ctx.getState()
        // We need to check there are any active chat, 
        // if had, we need to move it back to chat room
        if (ObjectUtils.isNotNull(state.activeChatSession)) {
            const foundRoom: DoubleChatRoom = ObjectUtils.clone(state.chatRooms.find(a => a.chatRoomId === state.activeChatSession.chatRoomId))
            foundRoom.currentSession = state.activeChatSession
            ctx.setState(
                patch({
                    chatRooms: updateItem<ChatRoom>(a => a.chatRoomId === foundRoom.chatRoomId, foundRoom),
                    activeChatRoom: null
                })
            )
        }
    }

    @Action(ChatActions.ActiveDoubleChatRoom)
    public activeDoubleChatRoom(ctx: StateContext<ChatStateModel>, { event }: ChatActions.ActiveDoubleChatRoom) {
        const state = ctx.getState()
        if (ObjectUtils.isNotNull(state.activeChatSession)
            && event.chatSession.chatRoomId != state.activeChatSession.chatRoomId) {
            // We need to bring current session of another
            let foundOldActiveRoom: DoubleChatRoom = ObjectUtils.clone(state.chatRooms.find(a => a.chatRoomId === state.activeChatSession.chatRoomId))
            foundOldActiveRoom.currentSession = {
                ...state.activeChatSession
            }

            // Bring another current session
            let foundNewActiveRoom = state.chatRooms.find(a => a.chatRoomId === event.chatSession.chatRoomId)
            foundNewActiveRoom = {
                ...foundNewActiveRoom,
                lastVisited: (new Date()).getDate()
            }
            ctx.setState(
                patch({
                    chatRooms: updateItem<ChatRoom>(a => a.chatRoomId === foundNewActiveRoom.chatRoomId, foundNewActiveRoom)
                })
            )
            return ctx.setState(
                patch({
                    activeChatSession: foundNewActiveRoom.currentSession,
                    chatRooms: updateItem<ChatRoom>(a => a.chatRoomId === foundOldActiveRoom.chatRoomId, foundOldActiveRoom)
                })
            )

        }
        else if (!ObjectUtils.isNotNull(state.activeChatSession)) {

            return ctx.setState(
                patch({
                    activeChatSession: event.chatSession
                })
            )
        }
    }

    @Action(ChatActions.FetchedNewDoubleChatRoom)
    public fetchedNewDoubleChatRoom(ctx: StateContext<ChatStateModel>, { event }: ChatActions.FetchedNewDoubleChatRoom) {
        const state = ctx.getState()
        // Only new chat room is reached this action        
        if (event.chatRoom.type === RoomType.Double) {
            event.chatRoom.lastVisited = (new Date()).getDate()
            ctx.setState(
                patch({
                    chatRooms: [
                        ...state.chatRooms,
                        event.chatRoom
                    ],
                    invitingUser: null
                })
            )

            if (ctx.getState().chatRooms.length > MAX_ROOMS) {
                ctx.dispatch(new ChatActions.RemoveLastLongActiveChatRoom(event.chatRoom.chatRoomId))
            }

            // Now we need to check the chat box is opened or not
            if (state.isOpenChatBox && ObjectUtils.isNotNull(state.activeChatSession)) {
                // Still have one active chat box, we should notify instead of activating this room
                return ctx.dispatch(new ChatActions.NotifyNewIncomingMessage({
                    chatRoomId: event.chatRoom.chatRoomId
                }))
            }
            else {
                // There are no open box, we can active this chat room
                return ctx.dispatch(new ChatActions.ActiveDoubleChatRoom({
                    chatSession: event.chatRoom.currentSession
                }))
            }
        }
    }

    @Action(ChatActions.LoadedMoreSession)
    public loadingMoreSession(ctx: StateContext<ChatStateModel>, { event }: ChatActions.LoadedMoreSession) {
        const state = ctx.getState()
        return ctx.setState(
            patch({
                activeChatSession: {
                    ...state.activeChatSession,
                    messages: [
                        ...event.chatSession.messages,
                        ...state.activeChatSession.messages
                    ],
                    previousSessionId: event.chatSession.previousSessionId
                }
            })
        )
    }

    @Action(ChatActions.AddedNewSession)
    public addedNewSession(ctx: StateContext<ChatStateModel>, { event }: ChatActions.AddedNewSession) {
        const state = ctx.getState()
        if (ObjectUtils.isNotNull(state.activeChatSession) && state.isOpenChatBox) {
            return ctx.setState(
                patch({
                    activeChatSession: {
                        ...state.activeChatSession,
                        sessionId: event.chatSession.sessionId,
                        previousSessionId: state.activeChatSession.previousSessionId // Keep last previous id
                    }
                })
            )
        }
        else {
            // If there are no active chat session, so that means this added new session coming
            // from unactived or unloaded 
            const foundRoom = state.chatRooms.find(a => a.chatRoomId === event.chatSession.chatRoomId)
            if (foundRoom) {
                let clonedFoundRoom: DoubleChatRoom = ObjectUtils.clone(foundRoom)
                clonedFoundRoom.currentSession.sessionId = event.chatSession.sessionId
                ctx.setState(
                    patch({
                        chatRooms: updateItem<ChatRoom>(a => a.chatRoomId === clonedFoundRoom.chatRoomId, clonedFoundRoom)
                    })
                )
            }
            else {
                // Do nothing because this chat room isn't ready to receive new session
            }
        }

    }

    @Action(ChatActions.SentMessage)
    public sentMessage(ctx: StateContext<ChatStateModel>, { event }: ChatActions.SentMessage) {
        const state = ctx.getState()
        if (event.chatSessionId === state.activeChatSession.sessionId) {
            const lastMessage = state.activeChatSession.messages[state.activeChatSession.messages.length - 1]
            if(lastMessage){
                event.message.renderTime = lastMessage.userName !== event.message.userName
            }
            else{
                event.message.renderTime = true
            }
            
            return ctx.setState(
                patch({
                    activeChatSession: {
                        ...state.activeChatSession,
                        messages: [
                            ...state.activeChatSession.messages,
                            event.message
                        ]
                    },
                    lastSentHashCode: event.lastSentHashCode
                })
            )
        }
    }

    @Action(ChatActions.ReceivedMessageFromAnotherDevice)
    public receivedFromAnotherDevice(ctx: StateContext<ChatStateModel>, { event }: ChatActions.ReceivedMessageFromAnotherDevice) {
        const state = ctx.getState()
        if (event.chatSessionId === state.activeChatSession.sessionId
            && state.lastSentHashCode !== event.lastHashCode) {
            const lastMessage = state.activeChatSession.messages[state.activeChatSession.messages.length - 1]
            event.message.renderTime = lastMessage.userName !== event.message.userName
            return ctx.setState(
                patch({
                    activeChatSession: {
                        ...state.activeChatSession,
                        messages: [
                            ...state.activeChatSession.messages,
                            event.message
                        ]
                    },
                    lastSentHashCode: event.lastHashCode
                })
            )
        }
    }

    @Action(ChatActions.NotifyNewIncomingMessage)
    public notifyIncomingMessage(ctx: StateContext<ChatStateModel>, { event }: ChatActions.NotifyNewIncomingMessage) {
        return ctx.setState(
            patch({
                notifiedChatRooms: insertItem<string>(event.chatRoomId)
            })
        )
    }

    @Action(ChatActions.ReceivedMessage)
    public receivedMessage(ctx: StateContext<ChatStateModel>, { event }: ChatActions.ReceivedMessage) {
        const state = ctx.getState()
        // In case a received message is sent to active chat room
        if (state.isOpenChatBox && event.chatRoomId === state.activeChatSession.chatRoomId
            && event.chatSessionId === state.activeChatSession.sessionId) {
            const lastMessage = state.activeChatSession.messages[state.activeChatSession.messages.length - 1]
            if(lastMessage){
                event.message.renderTime = lastMessage.userName !== event.message.userName
            }
            else{
                event.message.renderTime = true
            }
            
            return ctx.setState(
                patch({
                    activeChatSession: {
                        ...state.activeChatSession,
                        messages: [
                            ...state.activeChatSession.messages,
                            event.message
                        ]
                    }
                })
            )
        }
        else {
            // Find chat room is already there
            const foundRoom = state.chatRooms.find(a => a.chatRoomId === event.chatRoomId)
            if (ObjectUtils.isNotNull(foundRoom)) {
                if (foundRoom.type === RoomType.Double) {
                    // Add received message into current session of chat room
                    let clonedFoundRoom: DoubleChatRoom = ObjectUtils.clone(foundRoom)
                    const lastMessage = clonedFoundRoom.currentSession.messages[clonedFoundRoom.currentSession.messages.length - 1]
                    if(lastMessage){
                        event.message.renderTime = lastMessage.userName !== event.message.userName
                    }
                    else{
                        event.message.renderTime = true
                    }
                    
                    clonedFoundRoom.currentSession.messages.push(event.message)
                    // Ensure we still keep last session
                    clonedFoundRoom.currentSession.sessionId = event.chatSessionId

                    ctx.setState(
                        patch({
                            chatRooms: updateItem<ChatRoom>(a => a.chatRoomId === clonedFoundRoom.chatRoomId, clonedFoundRoom)
                        })
                    )
                    // Now notify for user
                    ctx.dispatch(new ChatActions.NotifyNewIncomingMessage({
                        chatRoomId: clonedFoundRoom.chatRoomId
                    }))
                }
            }
            else {
                // New change: we need to check current chat rooms is reached maximum or not
                // If it reached maximum rooms, we need to notify new chat instead of fecthing room
                if (state.chatRooms.length < MAX_ROOMS) {
                    const sender = state.availableUsers.find(a => a.userName === event.message.userName)
                    ctx.setState(
                        patch({
                            invitingUser: sender
                        })
                    )
                    // We need to fetch new chat room
                    ctx.dispatch(new ChatActions.FetchDoubleChatRoom())
                }
                else {
                    // If number of rooms is full, increase incoming messages
                    ctx.dispatch(new ChatActions.IncomingMessageFromUnloadUser({
                        sender: event.message.userName
                    }))
                }
            }
        }
    }

    @Action(ChatActions.LoadedAllAvailableUsers)
    public loadedAllUsers(ctx: StateContext<ChatStateModel>, { event }: ChatActions.LoadedAllAvailableUsers) {
        event.availableUsers?.forEach(u => {
            u.incomingMessages = 0
        })
        return ctx.setState(
            patch({
                availableUsers: event.availableUsers
            })
        )
    }

    @Action(ChatActions.IncomingOnlineUser)
    public incomingOnlineUser(ctx: StateContext<ChatStateModel>, { event }: ChatActions.IncomingOnlineUser) {
        const state = ctx.getState()
        const foundUser = state.availableUsers.find(a => a.userName === event.onlineUser.userName)
        if (ObjectUtils.isNotNull(foundUser)) {
            return ctx.setState(
                patch({
                    availableUsers: updateItem<ChatOnlineUser>(a => a.userName === event.onlineUser.userName, event.onlineUser)
                })
            )
        }
        else {
            return ctx.setState(
                patch({
                    availableUsers: insertItem<ChatOnlineUser>(event.onlineUser)
                })
            )
        }
    }

    @Action(ChatActions.IncomingOfflineUser)
    public incomingOfflineUser(ctx: StateContext<ChatStateModel>, { event }: ChatActions.IncomingOfflineUser) {
        const state = ctx.getState()
        let foundUser = state.availableUsers.find(a => a.userName === event.offlineUser)

        if (ObjectUtils.isNotNull(foundUser)) {
            foundUser = {
                ...foundUser,
                isOnline: false
            }
            return ctx.setState(
                patch({
                    availableUsers: updateItem<ChatOnlineUser>(a => a.userName === event.offlineUser, foundUser)
                })
            )
        }
    }

    @Action(ChatActions.IncomingMessageFromUnloadUser)
    public incomingUnloadUser(ctx: StateContext<ChatStateModel>, { event }: ChatActions.IncomingMessageFromUnloadUser) {
        const state = ctx.getState()
        let foundUser = state.availableUsers.find(a => a.userName === event.sender)

        if (ObjectUtils.isNotNull(foundUser)) {
            foundUser = {
                ...foundUser,
                incomingMessages: foundUser.incomingMessages + 1
            }
            return ctx.setState(
                patch({
                    availableUsers: updateItem<ChatOnlineUser>(a => a.userName === event.sender, foundUser)
                })
            )
        }
    }

    @Action(ChatActions.RemoveLastLongActiveChatRoom)
    public removeLastLongActiveRoom(ctx: StateContext<ChatStateModel>, { newChatRoomId }: ChatActions.RemoveLastLongActiveChatRoom) {
        const state = ctx.getState()
        let filtered = [...state.chatRooms.filter(a => a.chatRoomId !== state.activeChatSession.chatRoomId
            && a.chatRoomId !== newChatRoomId)]
        if (filtered.length > 0) {
            const lastLongActive = [...state.chatRooms.filter(a => a.chatRoomId !== state.activeChatSession.chatRoomId
                && a.chatRoomId !== newChatRoomId)].sort((user1, user2) => user1.lastVisited - user2.lastVisited)[0]
            return ctx.setState(
                patch({
                    chatRooms: removeItem<ChatRoom>(a => a.chatRoomId === lastLongActive.chatRoomId)
                })
            )
        }
    }

    @Action(ChatActions.NotifyIncomingVideoCall)
    public incomingVideoCall(ctx: StateContext<ChatStateModel>, { event }: ChatActions.NotifyIncomingVideoCall) {
        const foundUser = ctx.getState().availableUsers.find(a => a.userName === event.caller.username)
        return ctx.setState(
            patch({
                incomingVideoCall: event.caller,
                inviterVideoCall: foundUser
            })
        )
    }

    @Action(ChatActions.HandshakedVideoCall)
    public handshakeVideoCall(ctx: StateContext<ChatStateModel>, { event }: ChatActions.HandshakedVideoCall) {
        return ctx.setState(
            patch({
                handshakedVideoCall: event.videoRoom
            })
        )
    }

    @Action(ChatActions.ReceivedIceServer)
    public receivedIceServer(ctx: StateContext<ChatStateModel>, { event }: ChatActions.ReceivedIceServer) {
        return ctx.setState(
            patch({
                iceServers: event.iceServers
            })
        )
    }

    @Action(ChatActions.DroppedCall)
    public droppedCall(ctx: StateContext<ChatStateModel>, { }: ChatActions.DroppedCall) {
        return ctx.setState(
            patch({
                handshakedVideoCall: null,
                iceServer: null,
                incomingVideoCall: null,
                inviterVideoCall: null
            })
        )
    }

    @Action(ChatActions.ForceDroppedCall)
    public forceDroppedCall(ctx: StateContext<ChatStateModel>, { error }: ChatActions.ForceDroppedCall) {
        return ctx.setState(
            patch({
                handshakedVideoCall: null,
                iceServer: null,
                incomingVideoCall: null,
                inviterVideoCall: null,
                callErrorCode: error
            })
        )
    }
}