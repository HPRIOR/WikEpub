export class InputManager {
    constructor(parentNode) {
        this.parentNode = parentNode;
        this.nodeNum = 1;
        this.inputNodes = [];
        this.currentNode = document.getElementById('input-frame-1');
        this.inputNodes.push(this.currentNode);
    }
    insertInput() {
        if (this.nodeNum > 9)
            return null;
        this.nodeNum++;
        let insertNode = this.createInputNode();
        this.insertAfter(this.currentNode, insertNode);
        this.currentNode = insertNode;
        this.inputNodes.push(insertNode);
        return insertNode;
    }
    insertAfter(olderSibling, youngerSibling) {
        olderSibling.after(youngerSibling);
    }
    removeInput() {
        if (this.nodeNum === 1)
            return false;
        this.parentNode.removeChild(this.inputNodes.pop());
        this.currentNode = this.inputNodes[this.inputNodes.length - 1];
        this.nodeNum--;
        return true;
    }
    CreateInputNode() {
        let inputNode = document.createElement("input");
        inputNode.setAttribute("name", "WikiPages");
        inputNode.setAttribute("id", "input" + this.nodeNum.toString());
        inputNode.className = "url-input";
        return inputNode;
    }
    CreateEnclosingNode() {
        let enclosingNode = document.createElement("div");
        enclosingNode.id = "input-frame-" + this.nodeNum.toString();
        enclosingNode.setAttribute("class", "input-frame");
        return enclosingNode;
    }
    CreateCrossElement() {
        let span = document.createElement("span");
        span.setAttribute("class", "valid-status");
        span.textContent = '\u2718';
        span.id = "url-cross-" + this.nodeNum.toString();
        return span;
    }
    createInputNode() {
        let enclosingNode = this.CreateEnclosingNode();
        enclosingNode.appendChild(this.CreateInputNode());
        enclosingNode.appendChild(this.CreateCrossElement());
        return enclosingNode;
    }
}
//# sourceMappingURL=InputManager.js.map