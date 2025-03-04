import React from 'react'
import Editor from '@monaco-editor/react';
import {
  Menubar,
  MenubarContent,
  MenubarItem,
  MenubarMenu,
  MenubarShortcut,
  MenubarTrigger,
} from "@/components/ui/menubar"

interface TextEditorProps {
  code: string;
  setCode: (code: string) => void;
  handleExecute: () => void;
}

const TextEditor = ({ code, setCode, handleExecute}: TextEditorProps) => {

  const handleCreate = () => {
    console.log('create file');
  }

  const openFile = () => {
    console.log('open file');
  }

  const saveFile = () => {
    console.log('save file');
  }

  return (
    <div className='flex flex-col  justify-center bg-[#131313]  w-2/3 h-full ml-3'>
      <Menubar className='bg-[#1e1e1e] text-white p-3 mb-2 border-b border-[#333]'>
        <MenubarMenu>
          <MenubarTrigger>File</MenubarTrigger>
          <MenubarContent>
            <MenubarItem onClick={handleCreate}>
              New File <MenubarShortcut>⌘T</MenubarShortcut>
            </MenubarItem>
            <MenubarItem onClick={openFile}>
              Open File <MenubarShortcut>⌘O</MenubarShortcut>
            </MenubarItem>
            <MenubarItem onClick={saveFile}>
              Save File <MenubarShortcut>⌘S</MenubarShortcut>
            </MenubarItem>
          </MenubarContent>
        </MenubarMenu>
        <MenubarMenu>
          {/* compile */}
          <MenubarTrigger>Tools</MenubarTrigger>
          <MenubarContent>
            <MenubarItem onClick={handleExecute}>
              Execute <MenubarShortcut>⌘R</MenubarShortcut>
            </MenubarItem>
            <MenubarItem onClick={() => setCode('')}>
              Clear <MenubarShortcut>⌘K</MenubarShortcut>
            </MenubarItem>
          </MenubarContent>
        </MenubarMenu>
        <MenubarMenu>
          {/* compile */}
          <MenubarTrigger>Reports</MenubarTrigger>
          <MenubarContent>
            <MenubarItem onClick={() => console.log("errors clicked")}>
              Errors 
            </MenubarItem>
            <MenubarItem onClick={() => console.log("symbol clicked")}>
              Symbol Table 
            </MenubarItem>
            <MenubarItem onClick={() => console.log("ast clicked")}>
              Abstract Syntax Tree
            </MenubarItem>
          </MenubarContent>
        </MenubarMenu>
    </Menubar>
      <Editor
        height="80%"
        defaultLanguage="go"
        theme='vs-dark'
        value={code}
        onChange={(value) => value && setCode(value)}
        
    />
    </div>
    

  )
}

export default TextEditor