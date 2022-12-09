import { Injectable } from '@angular/core'
import { MatSnackBar } from '@angular/material/snack-bar'

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  constructor(private snackBar: MatSnackBar) {

  }

  showDefaultMessage(message: string, autoHide: boolean = true) {
    if (message) {
      this.openSnackBar(message, 'x', autoHide, 'default-message')
    }
  }

  showSuccessMessage(message: string, autoHide: boolean = true) {
    if (message) {
      this.openSnackBar(message, 'x', autoHide, 'success-message')
    }
  }

  showInfoMessage(message: string, autoHide: boolean = true) {
    if (message) {
      this.openSnackBar(message, 'x', autoHide, 'info-message')
    }
  }

  showErrorMessage(message: string, autoHide: boolean = true) {
    this.openSnackBar(message, 'x', autoHide, 'error-message')
  }

  private openSnackBar(message: string, action: string, autoHide: boolean = true, panelClass: string) {
    this.snackBar.open(message, action, {
      duration: autoHide ? 10000 : undefined,
      horizontalPosition: 'center',
      verticalPosition: 'top',
      politeness: 'polite',
      panelClass: ['mat-toolbar', panelClass]
    })
  }

  public playUserJoinEventTone() {
    const audio = new Audio()
    audio.src = './assets/audio/user-joined.mp3'
    audio.load()
    audio.play()
  }

  async getSampleAudioPlayer(): Promise<HTMLAudioElement> {
    const audio = new Audio()
    audio.src = './assets/audio/sample.mp3'
    audio.load()
    return audio
  }
}
