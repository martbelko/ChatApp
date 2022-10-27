import { Header } from './Components/Header/Header';
import { Contacts } from './Components/Contacts/Contacts';
import { Messages } from './Components/Messages/Messages';
import { Footer } from './Components/Footer/Footer';
import { Message } from './Components/Messages/Message';
import { useState } from 'react';
import { ConversationModel, MessageModel } from './Models';
import { useRecoilState, useRecoilValue } from 'recoil';
import { currentUserState } from './atoms';

function App() {
  const [currentUser, setCurrentUser] = useRecoilState(currentUserState);
  const [messages, setMessages] = useState<Array<MessageModel>>([]);

  const c: ConversationModel = {
    id: 1,
    messages: [],
    users: []
  }

  function addMessage(content: string) {
    const message: MessageModel = {
      id: 1,
      author: currentUser,
      content: content,
      datetime: new Date(Date.now()),
      conversation: c
    };
    const newMessages = [...messages, message];
    setMessages(newMessages);
  }

  return (
    <>
      <Header />
      <Contacts />
      <Messages addMessage={addMessage}>
        <Message authorId={1} content={'My first message'} datetime={new Date(Date.now())} />
      </Messages>
      <Footer />
    </>
  )
}

export default App;
