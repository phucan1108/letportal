import { Component, OnInit, ViewChild, ElementRef, Input, Output, EventEmitter, AfterViewInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
 
import { NGXLogger } from 'ngx-logger';

export class SuggestionHintOptions {
    class = ''
    startTriggerChars = '{{'
    endTriggerChars = '}}'
    allowCtrlSpace = true
    allowLongTouch = true
    numberChars = 2
}

@Component({
    selector: 'let-suggestion-hint',
    templateUrl: './suggestion-hint.component.html',
    styleUrls: ['./suggestion-hint.component.scss'],
    host: {
        '(document:click)': 'onClick($event)',
    }
})
export class SuggestionHintComponent implements OnInit, AfterViewInit {
    @ViewChild('formNameInput', { static: true }) formNameInput: ElementRef;
    @ViewChild('suggestionHint', { static: true }) suggestionHint: ElementRef;
    @ViewChild('spanTextWrap', { static: true }) spanTextWrap: ElementRef;

    @Input()
    options: SuggestionHintOptions

    @Input()
    dataSource: Array<string>

    @Output()
    change: EventEmitter<any> = new EventEmitter();

    @Input()
    formGroup: FormGroup

    @Input()
    controlKey: string

    @Input()
    placeholder: string

    appendedClass = ''
    left = '0px'
    visible = 'hidden'
    allowLongTouch = true
    allowCtrlSpace = true
    startChars = '{{'
    endChars = '}}'
    numberChars = 2
    tempText = ''
    tempSuggestionWord = ''
    suggestionList: Array<string> = []

    constructor(
        private fb: FormBuilder,
        private logger: NGXLogger
    ) { }

    ngOnInit(): void {
        if (this.options) {
            this.appendedClass = this.options.class
            this.allowCtrlSpace = this.options.allowCtrlSpace
            this.allowLongTouch = this.options.allowLongTouch
            this.startChars = this.options.startTriggerChars
            this.endChars = this.options.endTriggerChars
            this.numberChars = this.options.numberChars
        }
    }

    ngAfterViewInit(): void {
        this.populateValueChanges()
    }

    populateValueChanges() {
        this.formGroup.get(this.controlKey).valueChanges.subscribe((newValue: string) => {
            this.change.emit(newValue)
        })
    }

    onCtrlSpacePress($event: any) {
        // Combine between ctrl + space will display all suggestion list
        this.suggestionList = this.dataSource;
        this.displaySuggestionBox(true);
    }


    /**
     * Get the newValue change of the input and pop up the suggestion box
     * Expectation:
     * 1. When user types correct startChars, we will add the endChars and make the cursor in the middle
     * 2. When user types next numberChars (ex: 2 chars), we will trigger to check the suggestion box
     * params @event any
     */
    onKeyUp($event: any) {
        const currentCaretPos = this.formNameInput.nativeElement.selectionStart
        const inputControl = this.formGroup.get(this.controlKey)
        const currentInputValue = inputControl.value
        if ($event.code.toLowerCase() === 'space'
            || $event.code.toLowerCase() === 'delete'
            || $event.code.toLowerCase() === 'enter'
            || $event.code.toLowerCase() === 'tab'
            || $event.code.toLowerCase() === 'backspace') {
            this.displaySuggestionBox(false)
        }
        // Ignore action
        else if (
            $event.code.toLowerCase() === 'arrowleft'
            || $event.code.toLowerCase() === 'arrowright'
            || $event.code.toLowerCase() === 'arrowup'
            || $event.code.toLowerCase() === 'arrowdown') {
        }
        // Append endChars when user types startChars
        else if (currentInputValue.substr(currentCaretPos - this.startChars.length, this.startChars.length) === this.startChars
            && currentInputValue.substr(currentCaretPos, this.endChars.length) !== this.endChars) {
            inputControl.setValue([currentInputValue.slice(0, currentCaretPos), this.endChars, currentInputValue.slice(currentCaretPos)].join(''))
            this.setSelectionRange(this.formNameInput.nativeElement, currentCaretPos, currentCaretPos)
        }
        else {
            const word = this.getCurrentSuggestWord(currentInputValue, this.startChars, this.endChars, currentCaretPos, this.numberChars)
            if (word) {
                this.suggestionList = this.getSuggestionList(word)
                this.tempText = currentInputValue.substr(0, currentCaretPos)
                const enableSuggestion = this.suggestionList.length > 0;
                // Debounce 200ms
                setTimeout(() => {
                    this.displaySuggestionBox(enableSuggestion)
                    this.tempSuggestionWord = enableSuggestion ? word : ''
                }, 200)
            }
        }
    }

    getCurrentSuggestWord(text: string, startChars: string, endChars: string, currentPos: number, numberChars: number) {
        const leftPad = text.substr(0, currentPos)
        const lastStartChars = leftPad.lastIndexOf(startChars)
        // Ex: we have leftPad is
        // abc/{{to <- leftPad
        // 01234567 <- Index
        // 4 <- leftPad.length - lastStartChars Find number of chars between startChars indx to end of leftPad
        // 4 <- startChars.length + numberChars Find minimum number of chars including startChars to display suggestion Hint
        // if 4 >= 4 means we have enough word to look up suggestion hint
        if (lastStartChars >= 0 && (leftPad.length - lastStartChars) >= (startChars.length + numberChars)) {
            const word = leftPad.substr(lastStartChars + startChars.length, leftPad.length - (lastStartChars + startChars.length))
            this.logger.debug('suggest word:', word)
            return word
        }
        return null;
    }

    getSuggestionList(word: string) {
        const matchedList = this.dataSource.filter((elem: string) => { return elem.indexOf(word) === 0; })
        this.logger.debug(`Input word ${word}:`, matchedList)
        return matchedList
    }

    displaySuggestionBox(enableSuggestion: boolean) {
        if (enableSuggestion) {
            const maxLeftPos = this.formNameInput.nativeElement.offsetWidth - this.suggestionHint.nativeElement.offsetWidth;
            if (this.spanTextWrap.nativeElement.offsetWidth > maxLeftPos) {
                this.left = maxLeftPos + 'px';
            }
            else {
                this.left = (this.spanTextWrap.nativeElement.offsetWidth - 5) + 'px';
            }
        }
        this.visible = enableSuggestion ? 'visible' : 'hidden';
    }

    selectSuggestHint(suggestWord: string) {
        this.logger.debug('selected suggest word:', suggestWord)
        const currentCaretPos = this.formNameInput.nativeElement.selectionStart
        const inputControl = this.formGroup.get(this.controlKey)
        const currentInputValue = inputControl.value
        const rightPad = currentInputValue.substr(currentCaretPos, currentInputValue.length - currentCaretPos)
        const leftPad = currentInputValue.substr(0, currentCaretPos - this.tempSuggestionWord.length)
        const newLeftPad = leftPad + suggestWord
        const fullNewText = newLeftPad + rightPad
        inputControl.setValue(fullNewText)
        this.setSelectionRange(this.formNameInput.nativeElement, newLeftPad.length, newLeftPad.length)
        this.displaySuggestionBox(false)
    }

    setSelectionRange(input, selectionStart, selectionEnd) {
        if (input.setSelectionRange) {
            input.focus();
            input.setSelectionRange(selectionStart, selectionEnd);
        } else if (input.createTextRange) {
            const range = input.createTextRange();
            range.collapse(true);
            range.moveEnd('character', selectionEnd);
            range.moveStart('character', selectionStart);
            range.select();
        }
    }
    onClick($event) {
        if (!this.suggestionHint.nativeElement.contains(event.target)) {
            this.displaySuggestionBox(false)
        }
    }
}
