import { Header } from './Components/Header/Header';
import { Contacts } from './Components/Contacts/Contacts';
import { MessageProps, Messages } from './Components/Messages/Messages';
import { Footer } from './Components/Footer/Footer';
import { MessageModel, UserModel } from './Models';

function App() {
  let messages: Array<MessageProps> = new Array<MessageProps>;
  const m: MessageProps = {
    authorId: 1,
    content: "My first message",
    time: new Date(Date.now())
  };

  messages.push(m);

  console.log(messages);

  return (
    <>
      <Header />
      <Contacts />
      <Messages messages={messages} />
      <Footer />
    </>
  )
}

export default App;
