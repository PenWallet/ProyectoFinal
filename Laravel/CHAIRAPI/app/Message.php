<?php

namespace App;

use Illuminate\Database\Eloquent\Model;

class Message extends Model
{
    protected $table = 'Messages';
    public $primaryKey = 'ID';
    public $incrementing = true;
    public $timestamps = false;

    protected $fillable = [
        'ID', 'text', 'sender', 'receiver', 'date'
    ];

    protected $casts = [
        'date' => 'datetime'
    ];
}
