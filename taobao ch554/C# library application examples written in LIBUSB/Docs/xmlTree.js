var minusImg = new Image();
minusImg.src = "treeNode_Minus.gif";
var plusImg = new Image();
plusImg.src = "treeNode_Plus.gif";
var emptyImg = new Image();
emptyImg.src = "treeNode_Empty.gif";

function showBranch(branch){
	var objBranch = document.getElementById(branch).style;
	if(objBranch.display=="block")
		objBranch.display="none";
	else
		objBranch.display="block";
	swapFolder('I' + branch);
}
function showLink(linkTrigger){
    window.open('LibUsbDotNet.html#'+linkTrigger,'content');
	var objBranch = document.getElementById(linkTrigger).style;
	if(objBranch.display!="block")
	{
		objBranch.display="block";
    	swapFolder('I' + linkTrigger);
	}
}
function onMouseOverText(idText){
	objText = document.getElementById('T'+idText);
	objText.style.backgroundColor="#D0D0DD";
}
function onMouseOutText(idText){
	objText = document.getElementById('T'+idText);
	objText.style.backgroundColor="#f0f0ff";
}
function onMouseOverImg(idText){
	objImg = document.getElementById('I'+idText).firstChild;
	objImg.style.backgroundColor="#ffaaff";
}
function onMouseOutImg(idText){
	objImg = document.getElementById('I'+idText).firstChild;
	objImg.style.backgroundColor="#f0f0ff";
}
function swapFolder(img){
	objImg = document.getElementById(img).firstChild;
	if(objImg.src.indexOf('treeNode_Plus.gif')>-1)
		objImg.src = minusImg.src;
	else if(objImg.src.indexOf('treeNode_Minus.gif')>-1)
		objImg.src = plusImg.src;
	else
		objImg.src = emptyImg.src;
	
}
