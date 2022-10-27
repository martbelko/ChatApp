import { Header } from './Components/Header/Header';
import { Contacts } from './Components/Contacts/Contacts';
import { Messages } from './Components/Messages/Messages';
import { Footer } from './Components/Footer/Footer';
import { Message } from './Components/Messages/Message';
import { useEffect, useState } from 'react';
import { ConversationModel, MessageModel, UserModel } from './Models';
import { useRecoilState } from 'recoil';
import { localUserState } from './Atoms';
import { gql, useQuery } from '@apollo/client';

function App(): JSX.Element {
  const [localUser, setLocalUser] = useRecoilState(localUserState);

    const username = 'Laco';
    const query = gql`
        query {
            user(where: {username: {eq: "Laco"}}) {
            id
            conversations {
                id
                messages(order: { time: ASC }) {
                id
                author {
                    id
                    username
                }
                time
                content
                }
                members {
                id
                username
                }
            }
            }
        }
  `;

  const {error, loading, data} = useQuery(query);
  useEffect(() => {
    if (loading || error) {
      return;
    }

    try {
      const userInfo = data.user[0];
      const conversations: Array<ConversationModel> = userInfo.conversations.map((c: any) => { // Go through all the conversations of the user
        const messages: Array<MessageModel> = c.messages.map((m: any) => { // Messages belonging to the conversation
          const message: MessageModel = {
            id: m.id,
            content: m.content,
            datetime: new Date(m.time),
            author: {
              id: m.author.id,
              username: m.author.username
            }
          };

          return message;
        });

        const members: Array<UserModel> = c.members.map((m: any) => { // Users belonging to the conversation
          const member: UserModel = {
            id: m.id,
            username: m.username
          };

          return member;
        });

        const conv: ConversationModel = {
          id: c.id,
          messages: messages,
          members: members
        };

        return conv;
      });

      const user: UserModel = {
        id: userInfo.id,
        username: username,
        conversations: conversations
      };

      setLocalUser(user);
    } catch (e) {
      console.log(e);
    }
  }, [loading]);

    if (loading || !localUser) {
        return (<div>Loading...</div>);
    }

  /*function addMessage(content: string) {
    const activeConversation = currentUser.conversations![0];

    const message: MessageModel = {
        id: activeConversation.messages.length + 1,
        content: content,
        datetime: new Date(Date.now()),
        author: currentUser
    };

    const newConv: ConversationModel = {
        id: activeConversation.id,
        messages: [...activeConversation.messages, message],
        members: activeConversation!.members
    };

    console.log(newConv.messages);
    console.log(currentUser.conversations);

    const query = gql`
      mutation {
        addMessage(input: {
          userId: ${currentUser.id},
          conversationId: ${activeConversation?.id},
          content: ${content}
        })
        {
          message {
            id
            content
            time
          }, error
        }
      }
    `;

    const { error, loading, data } = useQuery(query);
    let message: MessageModel;
    if (loading) {
      message = {
        id: activeConversation!.messages.length + 1,
        author: currentUser,
        content: content,
        datetime: new Date(Date.now()),
        conversation: activeConversation
      };
    } else {
      const m = data.addMessage.message;
      message = {
        id: m.id,
        author: currentUser,
        content: content,
        datetime: new Date(Date.now()),
        conversation: activeConversation
      };

      console.log(data);
    }

    const newMessages = [...activeConversation!.messages, message];
    const newLocalUser: UserModel = {
      id: currentUser.id,
      username: currentUser.username,
      messages: newMessages,
      conversations: currentUser.conversations
    };

    setCurrentUser(newLocalUser);
  };*/

  return (
    <>
      <Header />
      <Contacts />
      <Messages activeConversation={localUser.conversations![0]} />
      <Footer />
    </>
  )
}

export default App;
